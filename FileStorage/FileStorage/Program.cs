using FileStorage.Data;
using FileStorage.Utils;
using FileStorage.Utils.Mappers;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// DB context
builder.Services.AddEntityFrameworkNpgsql()
    .AddDbContext<ApiDbContext>(opt => 
        opt.UseNpgsql(builder.Configuration.GetConnectionString("DatabaseConnection")).UseSnakeCaseNamingConvention());

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
            policy.WithOrigins("http://localhost:3000").AllowAnyHeader().AllowAnyMethod();
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

app.UseMiddleware<ExceptionHandingMiddleware>();

app.UseResponseCompression();

app.UseCors(MyAllowSpecificOrigins);

app.Run();
