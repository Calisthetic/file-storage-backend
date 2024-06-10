using Asp.Versioning;
using FileStorage.Data;
using FileStorage.Jobs;
using FileStorage.Services;
using FileStorage.Services.Mappers;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Quartz;
using Swashbuckle.AspNetCore.Filters;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using FileStorage.Main.Jobs;
using FileStorage.Middleware;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Hosting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

// JWT Auth
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
    });
    options.OperationFilter<SecurityRequirementsOperationFilter>();
});

var key = Encoding.ASCII.GetBytes(builder.Configuration.GetSection("JwtConfig:Secret").Value!);

var tokenValidationParameter = new TokenValidationParameters()
{
    ValidateIssuerSigningKey = true,
    IssuerSigningKey = new SymmetricSecurityKey(key),
    ValidateIssuer = false,
    ValidateAudience = false,
    RequireExpirationTime = false,
    ValidateLifetime = false,
    ClockSkew = TimeSpan.Zero,
};

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(jwt =>
{
    jwt.SaveToken = true;
    jwt.TokenValidationParameters = tokenValidationParameter;
});

builder.Services.AddSingleton(tokenValidationParameter);

// Versioning
builder.Services.AddApiVersioning(options =>
{
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.ReportApiVersions = true;
});

// Schedule jobs
builder.Services.AddQuartzHostedService(options =>
{
    options.WaitForJobsToComplete = true;
});

builder.Services.AddQuartz(q =>
{
    q.UseMicrosoftDependencyInjectionJobFactory();

    var jobKey = new JobKey(nameof(CalculateUsageJob));

    q.AddJob<CalculateUsageJob>(opts => opts.WithIdentity(jobKey));

    q.AddTrigger(opts => opts
    .ForJob(jobKey)
    .WithIdentity($"{jobKey}-trigger")
    .WithCronSchedule(builder.Configuration.GetSection("Jobs:CalculateUsageJob:CronSchedule").Value ?? "0 0 0 * * ?"));

    // Delete users
    var jobKey1 = new JobKey(nameof(DeleteUnverifiedUsersJob));

    q.AddJob<DeleteUnverifiedUsersJob>(opts => opts.WithIdentity(jobKey1));

    q.AddTrigger(opts => opts
    .ForJob(jobKey1)
    .WithIdentity($"{jobKey1}-trigger")
    .WithCronSchedule(builder.Configuration.GetSection("Jobs:DeleteUnverifiedUsersJob:CronSchedule").Value ?? "0 0 * * * ?"));
});

// Health check
builder.Services.AddHealthChecks()
    .AddNpgSql(builder.Configuration.GetConnectionString("DatabaseConnection")!);

// Read user claims
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IUserService, UserService>();
// Other services
builder.Services.AddScoped<IStatisticService, StatisticService>();

// Don't remove "Async" from names
builder.Services.AddMvc(options =>
{
    options.SuppressAsyncSuffixInActionNames = false;
});

// Don't send nullable values
bool EnableNulls = false;
builder.Configuration.GetSection("EnableNulls").Bind(EnableNulls);
builder.Services.AddControllers().AddJsonOptions(
    options => options.JsonSerializerOptions.DefaultIgnoreCondition = EnableNulls ? JsonIgnoreCondition.Never : JsonIgnoreCondition.WhenWritingNull
);

// Get config
var dbConfig = new DatabaseConfig();
builder.Configuration.GetSection("DatabaseConfig").Bind(dbConfig);

// DB context
builder.Services.AddDbContext<ApiDbContext>(opt => {
        opt.UseNpgsql(builder.Configuration.GetConnectionString("DatabaseConnection"), action =>
        {
            action.CommandTimeout(dbConfig.TimeoutTime);
        }).UseSnakeCaseNamingConvention();
        opt.EnableDetailedErrors(dbConfig.DetailedError);
        opt.EnableSensitiveDataLogging(dbConfig.SensitiveDataLogging);
    });

// Register mapper
builder.Services.AddMappings();

// Response compression
builder.Services.AddResponseCompression(options => options.EnableForHttps = true);

// Response style
builder.Services.AddControllersWithViews().AddJsonOptions(opt =>
{
    opt.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase; // null
});

// Remove max upload limit
builder.Services.Configure<FormOptions>(options =>
{
    // Set the limit to 512 MB
    options.MultipartBodyLengthLimit = 524288000;
});
builder.WebHost.ConfigureKestrel((context, options) =>
{
    options.Limits.MaxRequestBodySize = 524288000;
});

// CORS
//var MyAllowSpecificOrigins = "MyPolicy";
//builder.Services.AddCors(options =>
//{
//    options.AddPolicy(name: MyAllowSpecificOrigins,
//        policy =>
//        {
//            policy.WithOrigins("http://localhost:3000", "https://file-storage-frontend.vercel.app").AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin();
//        });
//});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// Added
//app.UseCors(MyAllowSpecificOrigins);

app.UseAuthentication();

app.MapHealthChecks("/healthz", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.UseMiddleware<ExceptionHandingMiddleware>();

app.UseResponseCompression();

app.Run();
