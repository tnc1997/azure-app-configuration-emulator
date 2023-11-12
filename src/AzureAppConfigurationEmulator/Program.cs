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

builder.WebHost.ConfigureKestrel();

var app = builder.Build();

app.MapGet("/kv/{key}", KeyValueHandler.Get);
app.MapGet("/kv", KeyValueHandler.List);
app.MapPut("/kv/{key}", KeyValueHandler.Set);
app.MapDelete("/kv/{key}", KeyValueHandler.Delete);
app.MapGet("/keys", KeyHandler.List);
app.MapGet("/labels", LabelHandler.List);
app.MapPut("/locks/{key}", LockHandler.Lock);
app.MapDelete("/locks/{key}", LockHandler.Unlock);

app.InitializeDatabase();

app.Run();
