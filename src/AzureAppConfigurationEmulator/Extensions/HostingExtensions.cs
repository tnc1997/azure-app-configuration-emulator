using System.Security.Cryptography.X509Certificates;
using AzureAppConfigurationEmulator.Contexts;
using Microsoft.EntityFrameworkCore;

namespace AzureAppConfigurationEmulator.Extensions;

public static class HostingExtensions
{
    public static string SslCertificatePath { get; } = Environment.OSVersion.Platform switch
    {
        PlatformID.Win32S => @"C:\ProgramData\Azure App Configuration Emulator\emulator.crt",
        PlatformID.Win32Windows => @"C:\ProgramData\Azure App Configuration Emulator\emulator.crt",
        PlatformID.Win32NT => @"C:\ProgramData\Azure App Configuration Emulator\emulator.crt",
        PlatformID.WinCE => @"C:\ProgramData\Azure App Configuration Emulator\emulator.crt",
        PlatformID.Unix => "/usr/local/share/azureappconfigurationemulator/emulator.crt",
        PlatformID.MacOSX => "/usr/local/share/azureappconfigurationemulator/emulator.crt",
        _ => throw new ArgumentOutOfRangeException()
    };

    public static string SslCertificateKeyPath { get; } = Environment.OSVersion.Platform switch
    {
        PlatformID.Win32S => @"C:\ProgramData\Azure App Configuration Emulator\emulator.key",
        PlatformID.Win32Windows => @"C:\ProgramData\Azure App Configuration Emulator\emulator.key",
        PlatformID.Win32NT => @"C:\ProgramData\Azure App Configuration Emulator\emulator.key",
        PlatformID.WinCE => @"C:\ProgramData\Azure App Configuration Emulator\emulator.key",
        PlatformID.Unix => "/usr/local/share/azureappconfigurationemulator/emulator.key",
        PlatformID.MacOSX => "/usr/local/share/azureappconfigurationemulator/emulator.key",
        _ => throw new ArgumentOutOfRangeException()
    };

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

    public static IWebHostBuilder ConfigureKestrel(this IWebHostBuilder builder)
    {
        return builder.ConfigureKestrel(options =>
        {
            if (File.Exists(SslCertificatePath) && File.Exists(SslCertificateKeyPath))
            {
                options.ConfigureHttpsDefaults(options =>
                {
                    options.ServerCertificate = X509Certificate2.CreateFromPemFile(SslCertificatePath, SslCertificateKeyPath);
                });
            }
        });
    }

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
