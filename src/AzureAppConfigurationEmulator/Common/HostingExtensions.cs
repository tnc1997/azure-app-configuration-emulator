using System.Security.Cryptography.X509Certificates;
using AzureAppConfigurationEmulator.Data;

namespace AzureAppConfigurationEmulator.Common;

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
        using var scope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope();
        var commandFactory = scope.ServiceProvider.GetRequiredService<IDbCommandFactory>();
        var connectionFactory = scope.ServiceProvider.GetRequiredService<IDbConnectionFactory>();

        using var connection = connectionFactory.Create();

        if (!Directory.Exists(Path.GetDirectoryName(connection.DataSource)!))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(connection.DataSource)!);
        }

        if (!File.Exists(connection.DataSource))
        {
            File.Create(connection.DataSource);
        }

        connection.Open();

        using var command = commandFactory.Create(connection);

        command.CommandText = """
                              CREATE TABLE IF NOT EXISTS configuration_settings (
                                  etag TEXT NOT NULL,
                                  key TEXT NOT NULL,
                                  label TEXT,
                                  content_type TEXT,
                                  value TEXT,
                                  last_modified TEXT NOT NULL,
                                  locked INTEGER NOT NULL,
                                  tags TEXT,
                                  PRIMARY KEY (key, label)
                              );

                              CREATE TABLE IF NOT EXISTS configuration_settings_history (
                                  etag TEXT NOT NULL,
                                  key TEXT NOT NULL,
                                  label TEXT,
                                  content_type TEXT,
                                  value TEXT,
                                  last_modified TEXT NOT NULL,
                                  locked INTEGER NOT NULL,
                                  tags TEXT,
                                  valid_from TEXT NOT NULL,
                                  valid_to TEXT NOT NULL
                              );

                              CREATE TRIGGER IF NOT EXISTS delete_configuration_setting
                                  AFTER DELETE ON configuration_settings
                                  FOR EACH ROW
                              BEGIN
                                  UPDATE configuration_settings_history
                                  SET valid_to = datetime()
                                  WHERE valid_to = '9999-12-31 23:59:59'
                                      AND key = old.key
                                      AND CASE old.label
                                              WHEN NOT NULL THEN label = old.label
                                              ELSE label IS NULL
                                          END;
                              END;

                              CREATE TRIGGER IF NOT EXISTS insert_configuration_setting
                                  AFTER INSERT ON configuration_settings
                                  FOR EACH ROW
                              BEGIN
                                  INSERT INTO configuration_settings_history (etag, key, label, content_type, value, last_modified, locked, tags, valid_from, valid_to)
                                  VALUES (new.etag, new.key, new.label, new.content_type, new.value, new.last_modified, new.locked, new.tags, new.last_modified, '9999-12-31 23:59:59');
                              END;

                              CREATE TRIGGER IF NOT EXISTS update_configuration_setting
                                  AFTER UPDATE ON configuration_settings
                                  FOR EACH ROW
                              BEGIN
                                  UPDATE configuration_settings_history
                                  SET valid_to = new.last_modified
                                  WHERE valid_to = '9999-12-31 23:59:59'
                                      AND key = old.key
                                      AND CASE old.label
                                              WHEN NOT NULL THEN label = old.label
                                              ELSE label IS NULL
                                          END;

                                  INSERT INTO configuration_settings_history (etag, key, label, content_type, value, last_modified, locked, tags, valid_from, valid_to)
                                  VALUES (new.etag, new.key, new.label, new.content_type, new.value, new.last_modified, new.locked, new.tags, new.last_modified, '9999-12-31 23:59:59');
                              END;
                              """;

        command.ExecuteNonQuery();
    }
}
