using System.Data.Common;

namespace AzureAppConfigurationEmulator.Factories;

public interface IDbParameterFactory
{
    public DbParameter Create<TValue>(string name, TValue? value);
}
