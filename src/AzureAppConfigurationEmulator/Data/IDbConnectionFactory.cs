using System.Data.Common;

namespace AzureAppConfigurationEmulator.Data;

public interface IDbConnectionFactory
{
    public DbConnection Create();
}
