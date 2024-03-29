using System.Data.Common;
using AzureAppConfigurationEmulator.Data.Abstractions;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;

namespace AzureAppConfigurationEmulator.Data.Sqlite;

public class SqliteDbConnectionFactory(IConfiguration? configuration = null) : IDbConnectionFactory
{
    private string ConnectionString { get; } = configuration?.GetConnectionString("DefaultConnection") ?? $"Data Source={DatabasePath}";

    private static string DatabasePath { get; } = Environment.OSVersion.Platform switch
    {
        PlatformID.Win32S => @"C:\ProgramData\Azure App Configuration Emulator\emulator.db",
        PlatformID.Win32Windows => @"C:\ProgramData\Azure App Configuration Emulator\emulator.db",
        PlatformID.Win32NT => @"C:\ProgramData\Azure App Configuration Emulator\emulator.db",
        PlatformID.WinCE => @"C:\ProgramData\Azure App Configuration Emulator\emulator.db",
        PlatformID.Unix => "/var/lib/azureappconfigurationemulator/emulator.db",
        PlatformID.MacOSX => "/var/lib/azureappconfigurationemulator/emulator.db",
        _ => throw new ArgumentOutOfRangeException()
    };

    public DbConnection Create()
    {
        return new SqliteConnection(ConnectionString);
    }
}
