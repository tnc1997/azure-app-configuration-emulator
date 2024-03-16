using AzureAppConfigurationEmulator.Constants;
using AzureAppConfigurationEmulator.Entities;

namespace AzureAppConfigurationEmulator.Repositories;

public interface IConfigurationSettingRepository
{
    public Task Add(
        ConfigurationSetting setting,
        CancellationToken cancellationToken = default);

    public IAsyncEnumerable<ConfigurationSetting> Get(
        string key = KeyFilter.Any,
        string label = LabelFilter.Any,
        DateTimeOffset? moment = default,
        CancellationToken cancellationToken = default);

    public Task Remove(
        ConfigurationSetting setting,
        CancellationToken cancellationToken = default);

    public Task Update(
        ConfigurationSetting setting,
        CancellationToken cancellationToken = default);
}
