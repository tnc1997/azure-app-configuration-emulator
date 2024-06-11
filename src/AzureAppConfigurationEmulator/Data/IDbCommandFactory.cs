using System.Data.Common;

namespace AzureAppConfigurationEmulator.Data;

public interface IDbCommandFactory
{
    public DbCommand Create(DbConnection connection, string? text = null, IEnumerable<DbParameter>? parameters = null);
}
