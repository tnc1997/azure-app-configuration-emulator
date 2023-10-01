using AzureAppConfigurationEmulator.Contexts;
using AzureAppConfigurationEmulator.Extensions;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(builder =>
{
    builder.UseSqlite($"Data Source={HostingExtensions.DatabasePath}");
});

var app = builder.Build();

app.MapGet("/", (ApplicationDbContext context) => context.ConfigurationSettings.AsAsyncEnumerable());

app.InitializeDatabase();

app.Run();
