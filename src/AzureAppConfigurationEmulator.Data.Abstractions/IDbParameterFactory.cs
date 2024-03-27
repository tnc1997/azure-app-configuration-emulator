using System.Data.Common;

namespace AzureAppConfigurationEmulator.Data.Abstractions;

public interface IDbParameterFactory
{
    public DbParameter Create<TValue>(string name, TValue? value);
}
