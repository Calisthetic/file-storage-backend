using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Ocelot.Middleware;
using Ocelot.DependencyInjection;
using System.Text;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//builder.Services.AddControllers();
//// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

////
//var key = Encoding.ASCII.GetBytes(builder.Configuration.GetSection("JwtConfig:Secret").Value!);

//var tokenValidationParameter = new TokenValidationParameters()
//{
//    ValidateIssuerSigningKey = true,
//    IssuerSigningKey = new SymmetricSecurityKey(key),
//    ValidateIssuer = false, // for dev
//    ValidateAudience = false, // for dev
//    RequireExpirationTime = false, // no refresh tokens
//    ValidateLifetime = false,
//    ClockSkew = TimeSpan.Zero,
//};

//var authenticationProviderKey = "TestKey";
//builder.Services.AddAuthentication(options =>
//{
//    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
//    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
//})
//.AddJwtBearer(authenticationProviderKey, jwt =>
//{
//    jwt.SaveToken = true;
//    jwt.TokenValidationParameters = tokenValidationParameter;
//});

//var app = builder.Build();

//// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}

//app.UseHttpsRedirection();

//app.UseAuthorization();

//app.MapControllers();

//app.Map("/swagger/v1/swagger.json", b =>
//{
//    b.Run(async x => {
//        var json = File.ReadAllText("swagger.json");
//        await x.Response.WriteAsync(json);
//    });
//});
//app.UseSwaggerUI(c =>
//{
//    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Ocelot");
//});

//app.UseOcelot().Wait();

//app.Run();


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Auth in swagger
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

//
var key = Encoding.ASCII.GetBytes(builder.Configuration.GetSection("JwtConfig:Secret").Value!);

//var tokenValidationParameter = new TokenValidationParameters()
//{
//    ValidateIssuerSigningKey = true,
//    IssuerSigningKey = new SymmetricSecurityKey(key),
//    ValidateIssuer = false, // for dev
//    ValidateAudience = false, // for dev
//    RequireExpirationTime = false, // no refresh tokens
//    ValidateLifetime = false,
//    ClockSkew = TimeSpan.Zero,
//};

//var authenticationProviderKey = "TestKey";
//builder.Services.AddAuthentication()
//.AddJwtBearer(authenticationProviderKey);

var app = new WebHostBuilder();
app
.UseKestrel()
.UseContentRoot(Directory.GetCurrentDirectory())
.ConfigureAppConfiguration((hostingContext, config) =>
{
    config
        .SetBasePath(hostingContext.HostingEnvironment.ContentRootPath)
        .AddJsonFile("appsettings.json", true, true)
        .AddJsonFile($"appsettings.{hostingContext.HostingEnvironment.EnvironmentName}.json", true, true)
        .AddJsonFile("ocelot.json")
        .AddEnvironmentVariables();
})
.ConfigureServices(s =>
{// Adding Authentication
    var baseAuthenticationProviderKey = "TestKey";

    s.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    })

    // Adding Jwt Bearer  
    .AddJwtBearer(baseAuthenticationProviderKey, options =>
    {
        options.SaveToken = true;
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = false, // for dev
            ValidateAudience = false, // for dev
            RequireExpirationTime = false, // no refresh tokens
            ValidateLifetime = false,
            ClockSkew = TimeSpan.Zero,
        };
    });
    s.AddOcelot();
})
.ConfigureLogging((hostingContext, logging) =>
{
    //add your logging
})
.UseIISIntegration()
.Configure(app =>
{
    app.Map("/swagger/v1/swagger.json", b =>
    {
        b.Run(async x =>
        {
            var json = File.ReadAllText("swagger.json");
            await x.Response.WriteAsync(json);
        });
    });
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Ocelot");
    });

    app.UseOcelot().Wait();
})
.Build()
.Run();