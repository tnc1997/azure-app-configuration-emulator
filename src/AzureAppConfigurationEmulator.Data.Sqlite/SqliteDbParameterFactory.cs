using System.Data.Common;
using System.Text.Json;
using AzureAppConfigurationEmulator.Data.Abstractions;
using Microsoft.Data.Sqlite;

namespace AzureAppConfigurationEmulator.Data.Sqlite;

public class SqliteDbParameterFactory : IDbParameterFactory
{
    public DbParameter Create<TValue>(string name, TValue? value)
    {
        return value switch
        {
            bool b => new SqliteParameter(name, SqliteType.Integer) { Value = b ? 1 : 0 },
            DateTime d => new SqliteParameter(name, SqliteType.Text) { Value = d.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss") },
            DateTimeOffset d => new SqliteParameter(name, SqliteType.Text) { Value = d.UtcDateTime.ToString("yyyy-MM-dd HH:mm:ss") },
            IDictionary<string, object?> d => new SqliteParameter(name, SqliteType.Text) { Value = JsonSerializer.Serialize(d) },
            int i => new SqliteParameter(name, SqliteType.Integer) { Value = i },
            null => new SqliteParameter(name, SqliteType.Text) { Value = DBNull.Value },
            string s => new SqliteParameter(name, SqliteType.Text) { Value = s },
            _ => new SqliteParameter(name, SqliteType.Text) { Value = value.ToString() }
        };
    }
}
