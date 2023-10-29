using System.Text.Json;
using AzureAppConfigurationEmulator.Contexts;
using AzureAppConfigurationEmulator.Extensions;
using AzureAppConfigurationEmulator.Handlers;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(builder =>
{
    builder.UseSqlite($"Data Source={HostingExtensions.DatabasePath}");
});

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower;
});

var app = builder.Build();

app.MapGet("/kv", KeyValueHandler.List);
app.MapPut("/kv/{key}", KeyValueHandler.Set);

app.InitializeDatabase();

app.Run();
