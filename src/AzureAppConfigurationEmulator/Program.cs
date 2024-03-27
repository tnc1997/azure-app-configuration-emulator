using System.Text.Json;
using AzureAppConfigurationEmulator.Authentication.Hmac;
using AzureAppConfigurationEmulator.Common;
using AzureAppConfigurationEmulator.Common.Abstractions;
using AzureAppConfigurationEmulator.Components;
using AzureAppConfigurationEmulator.ConfigurationSettings;
using AzureAppConfigurationEmulator.Data.Abstractions;
using AzureAppConfigurationEmulator.Data.Sqlite;
using AzureAppConfigurationEmulator.Extensions;
using AzureAppConfigurationEmulator.Keys;
using AzureAppConfigurationEmulator.Labels;
using AzureAppConfigurationEmulator.Locks;
using AzureAppConfigurationEmulator.Services;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddOpenTelemetry(options =>
{
    options.AddOtlpExporter();
});

builder.Services.AddAuthentication().AddHmac();

builder.Services.AddAuthorization();

builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource =>
    {
        resource.AddService(builder.Environment.ApplicationName);
    })
    .WithMetrics(metrics =>
    {
        metrics.AddAspNetCoreInstrumentation();
        metrics.AddOtlpExporter();
    })
    .WithTracing(tracing =>
    {
        tracing.AddAspNetCoreInstrumentation();
        tracing.AddSource(Telemetry.ActivitySource.Name);
        tracing.AddOtlpExporter();
    });

builder.Services.AddRazorComponents().AddInteractiveServerComponents();

builder.Services.AddScoped<IDialogService, DialogService>();

builder.Services.AddSingleton<IConfigurationSettingFactory, ConfigurationSettingFactory>();
builder.Services.AddSingleton<IConfigurationSettingRepository, ConfigurationSettingRepository>();
builder.Services.AddSingleton<IDbCommandFactory, SqliteDbCommandFactory>();
builder.Services.AddSingleton<IDbConnectionFactory, SqliteDbConnectionFactory>();
builder.Services.AddSingleton<IDbParameterFactory, SqliteDbParameterFactory>();

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower;
});

builder.WebHost.ConfigureKestrel();

var app = builder.Build();

app.Map("/_explorer", app =>
{
    app.UseRouting();
    app.UseStaticFiles();
    app.UseAntiforgery();

    app.UseEndpoints(endpoints =>
    {
        endpoints.MapRazorComponents<App>().AddInteractiveServerRenderMode();
    });
});

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/kv/{**key}", ConfigurationSettingHandler.Get).RequireAuthorization();
app.MapGet("/kv", ConfigurationSettingHandler.List).RequireAuthorization();
app.MapPut("/kv/{**key}", ConfigurationSettingHandler.Set).RequireAuthorization();
app.MapDelete("/kv/{**key}", ConfigurationSettingHandler.Delete).RequireAuthorization();
app.MapGet("/keys", KeyHandler.List).RequireAuthorization();
app.MapGet("/labels", LabelHandler.List).RequireAuthorization();
app.MapPut("/locks/{**key}", LockHandler.Lock).RequireAuthorization();
app.MapDelete("/locks/{**key}", LockHandler.Unlock).RequireAuthorization();

app.InitializeDatabase();

app.Run();
