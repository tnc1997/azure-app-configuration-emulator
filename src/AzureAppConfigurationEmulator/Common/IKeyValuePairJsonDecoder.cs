using System.Text.Json;

namespace AzureAppConfigurationEmulator.Common;

public interface IKeyValuePairJsonDecoder
{
    IEnumerable<KeyValuePair<string, string?>> Decode(
        JsonDocument document,
        string? prefix = null,
        string? separator = null);
}
