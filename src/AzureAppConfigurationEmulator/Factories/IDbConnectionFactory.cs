using System.Data.Common;

namespace AzureAppConfigurationEmulator.Factories;

public interface IDbConnectionFactory
{
    public DbConnection Create();
}
