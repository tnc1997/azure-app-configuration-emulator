using System.Security.Cryptography.X509Certificates;
using AzureAppConfigurationEmulator.Contexts;
using Microsoft.EntityFrameworkCore;

namespace AzureAppConfigurationEmulator.Extensions;

public static class HostingExtensions
{
    public static string SslCertificatePath { get; } = Environment.GetEnvironmentVariable("SSL_CERTIFICATE_CERT_PATH") ?? "data/emulator.crt";

    public static string SslCertificateKeyPath { get; } = Environment.GetEnvironmentVariable("SSL_CERTIFICATE_KEY_PATH") ?? "data/emulator.key";

    public static string DatabasePath { get; } = Environment.GetEnvironmentVariable("DATABASE_PATH") ?? "data/emulator.db";

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
