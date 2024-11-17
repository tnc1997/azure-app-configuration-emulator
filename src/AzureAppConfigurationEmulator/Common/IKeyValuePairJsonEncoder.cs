using System.Text.Json;

namespace AzureAppConfigurationEmulator.Common;

public interface IKeyValuePairJsonEncoder
{
    JsonDocument Encode(
        IEnumerable<KeyValuePair<string, string?>> pairs,
        string? prefix = null,
        string? separator = null);
}
