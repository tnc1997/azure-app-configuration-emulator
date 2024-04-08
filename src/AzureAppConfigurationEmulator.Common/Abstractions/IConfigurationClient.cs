using AzureAppConfigurationEmulator.Common.Constants;
using AzureAppConfigurationEmulator.Common.Models;

namespace AzureAppConfigurationEmulator.Common.Abstractions;

public interface IConfigurationClient
{
    public IAsyncEnumerable<ConfigurationSetting> GetConfigurationSettings(
        string key = KeyFilter.Any,
        string label = LabelFilter.Any,
        DateTimeOffset? moment = default,
        CancellationToken cancellationToken = default);

    public IAsyncEnumerable<string> GetKeys(
        CancellationToken cancellationToken = default);

    public IAsyncEnumerable<string?> GetLabels(
        CancellationToken cancellationToken = default);
}
