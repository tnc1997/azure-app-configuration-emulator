using System.Data.Common;

namespace AzureAppConfigurationEmulator.Data.Abstractions;

public interface IDbConnectionFactory
{
    public DbConnection Create();
}
