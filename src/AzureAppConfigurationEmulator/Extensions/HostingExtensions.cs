using AzureAppConfigurationEmulator.Contexts;
using Microsoft.EntityFrameworkCore;

namespace AzureAppConfigurationEmulator.Extensions;

public static class HostingExtensions
{
    public static string DatabasePath { get; } = Environment.OSVersion.Platform switch
    {
        PlatformID.Win32S => @"C:\ProgramData\Azure App Configuration Emulator\emulator.db",
        PlatformID.Win32Windows => @"C:\ProgramData\Azure App Configuration Emulator\emulator.db",
        PlatformID.Win32NT => @"C:\ProgramData\Azure App Configuration Emulator\emulator.db",
        PlatformID.WinCE => @"C:\ProgramData\Azure App Configuration Emulator\emulator.db",
        PlatformID.Unix => "/var/lib/azureappconfigurationemulator/emulator.db",
        PlatformID.MacOSX => "/var/lib/azureappconfigurationemulator/emulator.db",
        _ => throw new ArgumentOutOfRangeException()
    };

    public static void InitializeDatabase(this IApplicationBuilder app)
    {
        if (!Directory.Exists(Path.GetDirectoryName(DatabasePath)!))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(DatabasePath)!);
        }

        if (!File.Exists(DatabasePath))
        {
            File.Create(DatabasePath);
        }

        using (var scope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
        {
            scope.ServiceProvider.GetRequiredService<ApplicationDbContext>().Database.Migrate();
        }
    }
}
