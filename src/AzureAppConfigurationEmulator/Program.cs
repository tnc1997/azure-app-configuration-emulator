using System.Text.Json;
using AzureAppConfigurationEmulator.Authentication;
using AzureAppConfigurationEmulator.Contexts;
using AzureAppConfigurationEmulator.Extensions;
using AzureAppConfigurationEmulator.Handlers;
using AzureAppConfigurationEmulator.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication().AddHmac();

builder.Services.AddAuthorization();

builder.Services.AddDbContext<ApplicationDbContext>(builder =>
{
    builder.UseSqlite($"Data Source={HostingExtensions.DatabasePath}");
});

builder.Services.AddScoped<IConfigurationSettingRepository, ConfigurationSettingRepository>();

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower;
});

builder.WebHost.ConfigureKestrel();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/kv/{**key}", KeyValueHandler.Get).RequireAuthorization();
app.MapGet("/kv", KeyValueHandler.List).RequireAuthorization();
app.MapPut("/kv/{**key}", KeyValueHandler.Set).RequireAuthorization();
app.MapDelete("/kv/{**key}", KeyValueHandler.Delete).RequireAuthorization();
app.MapGet("/keys", KeyHandler.List).RequireAuthorization();
app.MapGet("/labels", LabelHandler.List).RequireAuthorization();
app.MapPut("/locks/{**key}", LockHandler.Lock).RequireAuthorization();
app.MapDelete("/locks/{**key}", LockHandler.Unlock).RequireAuthorization();

app.InitializeDatabase();

app.Run();
