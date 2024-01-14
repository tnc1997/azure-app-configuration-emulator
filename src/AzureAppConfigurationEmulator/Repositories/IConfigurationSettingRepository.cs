using AzureAppConfigurationEmulator.Constants;
using AzureAppConfigurationEmulator.Entities;

namespace AzureAppConfigurationEmulator.Repositories;

public interface IConfigurationSettingRepository
{
    public Task AddAsync(
        ConfigurationSetting setting,
        CancellationToken cancellationToken = default);

    public IAsyncEnumerable<ConfigurationSetting> Get(
        string key = KeyFilter.Any,
        string label = LabelFilter.Any,
        CancellationToken cancellationToken = default);

    public Task RemoveAsync(
        ConfigurationSetting setting,
        CancellationToken cancellationToken = default);

    public Task UpdateAsync(
        ConfigurationSetting setting,
        CancellationToken cancellationToken = default);
}
