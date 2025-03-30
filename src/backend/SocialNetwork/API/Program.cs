using Application.Repositories;
using Application.Services;
using Application.Services.Interfaces;
using Infrastructure;
using Infrastructure.Enrichers;
using Infrastructure.Repository;
using Serilog;
using Serilog.Sinks.Graylog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IProfileRepository, ProfileRepository>();
builder.Services.AddScoped<IPostRepository, PostRepository>();
builder.Services.AddScoped<IPostService, PostService>();
builder.Services.AddScoped<IProfileService, ProfileService>();
builder.Services.RegisterDb();

builder.Logging.ClearProviders();
builder.Host.UseSerilog((hostingContext, loggerConfiguration) =>
{
    loggerConfiguration.ReadFrom.Configuration(builder.Configuration);
    loggerConfiguration.Enrich.WithProperty("ServiceName", Environment.GetEnvironmentVariable("SERVICE_NAME") ?? "None");
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthorization();

app.MapControllers();

app.Run();