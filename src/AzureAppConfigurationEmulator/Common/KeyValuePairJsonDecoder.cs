using System.Globalization;
using System.Text.Json;

namespace AzureAppConfigurationEmulator.Common;

public class KeyValuePairJsonDecoder : IKeyValuePairJsonDecoder
{
    public IEnumerable<KeyValuePair<string, string?>> Decode(
        JsonDocument document,
        string? prefix = null,
        string? separator = null)
    {
        return Decode(document.RootElement, prefix, separator);
    }

    private IEnumerable<KeyValuePair<string, string?>> Decode(
        JsonElement element,
        string? prefix = null,
        string? separator = null)
    {
        using var activity = Telemetry.ActivitySource.StartActivity($"{nameof(KeyValuePairJsonDecoder)}.{nameof(Decode)}");

        switch (element.ValueKind)
        {
            case JsonValueKind.Object:
                foreach (var innerProperty in element.EnumerateObject())
                {
                    var innerPrefix = !string.IsNullOrEmpty(prefix) ? $"{prefix}{separator}{innerProperty.Name}" : innerProperty.Name;

                    foreach (var setting in Decode(innerProperty.Value, innerPrefix, separator))
                    {
                        yield return setting;
                    }
                }

                break;
            case JsonValueKind.Array:
                var index = 0;

                foreach (var innerElement in element.EnumerateArray())
                {
                    var innerPrefix = !string.IsNullOrEmpty(prefix) ? $"{prefix}{separator}{index}" : index.ToString();

                    foreach (var setting in Decode(innerElement, innerPrefix, separator))
                    {
                        yield return setting;
                    }

                    index += 1;
                }

                break;
            default:
                if (!string.IsNullOrEmpty(prefix))
                {
                    var value = element.ValueKind switch
                    {
                        JsonValueKind.Undefined => null,
                        JsonValueKind.Object => null,
                        JsonValueKind.Array => null,
                        JsonValueKind.String => element.GetString(),
                        JsonValueKind.Number => element.GetDouble().ToString(CultureInfo.InvariantCulture),
                        JsonValueKind.True => true.ToString(),
                        JsonValueKind.False => false.ToString(),
                        JsonValueKind.Null => null,
                        _ => null
                    };

                    yield return new KeyValuePair<string, string?>(prefix, value);
                }

                break;
        }
    }
}
