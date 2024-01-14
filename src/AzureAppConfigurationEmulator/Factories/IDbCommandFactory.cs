using System.Data.Common;

namespace AzureAppConfigurationEmulator.Factories;

public interface IDbCommandFactory
{
    public DbCommand Create(DbConnection connection, string? text = null, IEnumerable<DbParameter>? parameters = null);
}
