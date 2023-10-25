using Asp.Versioning;
using FileStorage.Data;
using FileStorage.Jobs;
using FileStorage.Services;
using FileStorage.Services.Mappers;
using FileStorage.Utils;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NpgsqlTypes;
using Quartz;
using Serilog.Sinks.PostgreSQL;
using Serilog;
using Swashbuckle.AspNetCore.Filters;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Serilog.Sinks.PostgreSQL.ColumnWriters;
using FileStorage.Main.Jobs;

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
    ValidateIssuer = false, // for dev
    ValidateAudience = false, // for dev
    RequireExpirationTime = false, // no refresh tokens
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

// Serilog to PortgreSQL
IDictionary<string, ColumnWriterBase> columnWriters = new Dictionary<string, ColumnWriterBase>
{
    { "message", new RenderedMessageColumnWriter(NpgsqlDbType.Text) },
    { "message_template", new MessageTemplateColumnWriter(NpgsqlDbType.Text) },
    { "level", new LevelColumnWriter(true, NpgsqlDbType.Varchar) },
    { "raise_date", new TimestampColumnWriter(NpgsqlDbType.TimestampTz) },
    { "exception", new ExceptionColumnWriter(NpgsqlDbType.Text) },
    { "properties", new LogEventSerializedColumnWriter(NpgsqlDbType.Jsonb) },
    { "props_test", new PropertiesColumnWriter(NpgsqlDbType.Jsonb) },
    { "machine_name", new SinglePropertyColumnWriter("MachineName", PropertyWriteMethod.ToString, NpgsqlDbType.Text, "l") }
};
var logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.PostgreSQL(builder.Configuration.GetConnectionString("DatabaseConnection")!.ToString(), "logs", columnWriters, needAutoCreateTable: true)
    .CreateLogger();
Log.Information("Start!"); 
builder.Logging.ClearProviders();
builder.Logging.AddSerilog(logger);

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

// CORS
var MyAllowSpecificOrigins = "MyPolicy";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
        policy =>
        {
            policy.WithOrigins("http://localhost:3000", "https://file-storage-frontend.vercel.app").AllowAnyHeader().AllowAnyMethod();
        });
});

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
app.UseAuthentication();

app.MapHealthChecks("/healthz", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.UseMiddleware<ExceptionHandingMiddleware>();

app.UseResponseCompression();

app.UseCors(MyAllowSpecificOrigins);

app.Run();
