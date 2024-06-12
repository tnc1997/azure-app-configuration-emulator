using System.Text;
using System.Text.Json;

namespace AzureAppConfigurationEmulator.ConfigurationSettings;

/// <summary>
/// https://github.com/Azure/azure-sdk-for-net/blob/main/sdk/appconfiguration/Azure.Data.AppConfiguration/src/Models/FeatureFlagConfigurationSetting.cs
/// </summary>
public class FeatureFlagConfigurationSetting : ConfigurationSetting
{
    public FeatureFlagConfigurationSetting(
        string id,
        bool enabled,
        ICollection<FeatureFlagFilter> clientFilters,
        string etag,
        string key,
        DateTimeOffset lastModified,
        bool locked,
        string? description = null,
        string? displayName = null,
        string? label = null,
        string? contentType = null,
        IDictionary<string, string>? tags = null) : base(
        etag,
        key,
        lastModified,
        locked,
        label: label,
        contentType: contentType,
        tags: tags)
    {
        Id = id;
        Enabled = enabled;
        ClientFilters = clientFilters;
        Description = description;
        DisplayName = displayName;
    }

    public FeatureFlagConfigurationSetting(
        string etag,
        string key,
        string value,
        DateTimeOffset lastModified,
        bool locked,
        string? label = null,
        string? contentType = null,
        IDictionary<string, string>? tags = null) : base(
        etag,
        key,
        lastModified,
        locked,
        label: label,
        contentType: contentType,
        tags: tags)
    {
        using var document = JsonDocument.Parse(value);

        Id = document.RootElement.GetProperty("id").GetString()!;

        Enabled = document.RootElement.GetProperty("enabled").GetBoolean();

        ClientFilters = document.RootElement.TryGetProperty("conditions", out var conditions) &&
                        conditions.TryGetProperty("client_filters", out var clientFilters) &&
                        clientFilters.ValueKind is JsonValueKind.Array
            ? clientFilters
                .EnumerateArray()
                .Select(clientFilter => new FeatureFlagFilter(clientFilter))
                .ToList()
            : [];

        Description = document.RootElement.TryGetProperty("description", out var description)
            ? description.GetString()
            : null;

        DisplayName = document.RootElement.TryGetProperty("display_name", out var displayName)
            ? displayName.GetString()
            : null;
    }

    public override string? Value
    {
        get
        {
            using var stream = new MemoryStream();
            using var writer = new Utf8JsonWriter(stream);

            writer.WriteStartObject();

            writer.WriteString("id", Id);

            writer.WriteBoolean("enabled", Enabled);

            writer.WriteStartObject("conditions");
            writer.WriteStartArray("client_filters");

            foreach (var clientFilter in ClientFilters)
            {
                clientFilter.WriteTo(writer);
            }

            writer.WriteEndArray();
            writer.WriteEndObject();

            if (Description is not null)
            {
                writer.WriteString("description", Description);
            }

            if (DisplayName is not null)
            {
                writer.WriteString("display_name", DisplayName);
            }

            writer.WriteEndObject();
            writer.Flush();

            return Encoding.UTF8.GetString(stream.ToArray());
        }
        set
        {
            ArgumentNullException.ThrowIfNull(value, nameof(value));

            using var document = JsonDocument.Parse(value);

            Id = document.RootElement.GetProperty("id").GetString()!;

            Enabled = document.RootElement.GetProperty("enabled").GetBoolean();

            ClientFilters = document.RootElement.TryGetProperty("conditions", out var conditions) &&
                            conditions.TryGetProperty("client_filters", out var clientFilters) &&
                            clientFilters.ValueKind is JsonValueKind.Array
                ? clientFilters
                    .EnumerateArray()
                    .Select(clientFilter => new FeatureFlagFilter(clientFilter))
                    .ToList()
                : [];

            Description = document.RootElement.TryGetProperty("description", out var description)
                ? description.GetString()
                : null;

            DisplayName = document.RootElement.TryGetProperty("display_name", out var displayName)
                ? displayName.GetString()
                : null;
        }
    }

    public string Id { get; set; }

    public bool Enabled { get; set; }

    public ICollection<FeatureFlagFilter> ClientFilters { get; set; }

    public string? Description { get; set; }

    public string? DisplayName { get; set; }
}

/// <summary>
/// https://github.com/Azure/azure-sdk-for-net/blob/main/sdk/appconfiguration/Azure.Data.AppConfiguration/src/Models/FeatureFlagFilter.cs
/// </summary>
public class FeatureFlagFilter
{
    public FeatureFlagFilter(string name, IDictionary<string, object> parameters)
    {
        Name = name;
        Parameters = parameters;
    }

    internal FeatureFlagFilter(JsonElement element)
    {
        Name = element.GetProperty("name").GetString()!;

        Parameters = element.TryGetProperty("parameters", out var parameters) &&
                     DeserializeParameters(parameters) is IDictionary<string, object> dictionary
                ? dictionary
                : new Dictionary<string, object>();
    }

    public string Name { get; set; }

    public IDictionary<string, object> Parameters { get; set; }

    internal void WriteTo(Utf8JsonWriter writer)
    {
        writer.WriteStartObject();

        writer.WriteString("name", Name);

        writer.WritePropertyName("parameters");
        WriteParametersValue(writer, Parameters);

        writer.WriteEndObject();
    }

    private static object? DeserializeParameters(JsonElement element)
    {
        return element.ValueKind switch
        {
            JsonValueKind.Object => element
                .EnumerateObject()
                .ToDictionary(
                    property => property.Name,
                    property => DeserializeParameters(property.Value)),
            JsonValueKind.Array => element
                .EnumerateArray()
                .Select(DeserializeParameters)
                .ToList(),
            JsonValueKind.String => element.GetString(),
            JsonValueKind.Number when element.TryGetInt32(out var value) => value,
            JsonValueKind.Number when element.TryGetInt64(out var value) => value,
            JsonValueKind.Number when element.TryGetDouble(out var value) => value,
            JsonValueKind.True or JsonValueKind.False => element.GetBoolean(),
            JsonValueKind.Undefined or JsonValueKind.Null => null,
            _ => throw new ArgumentOutOfRangeException(nameof(element), element.ValueKind, null)
        };
    }

    private static void WriteParametersValue(Utf8JsonWriter writer, object? value)
    {
        switch (value)
        {
            case null:
                writer.WriteNullValue();
                break;
            case int i:
                writer.WriteNumberValue(i);
                break;
            case double d:
                writer.WriteNumberValue(d);
                break;
            case float f:
                writer.WriteNumberValue(f);
                break;
            case long l:
                writer.WriteNumberValue(l);
                break;
            case string s:
                writer.WriteStringValue(s);
                break;
            case bool b:
                writer.WriteBooleanValue(b);
                break;
            case IDictionary<string, object> dictionary:
                writer.WriteStartObject();

                foreach (var pair in dictionary)
                {
                    writer.WritePropertyName(pair.Key);
                    WriteParametersValue(writer, pair.Value);
                }

                writer.WriteEndObject();

                break;
            case IEnumerable<object> enumerable:
                writer.WriteStartArray();

                foreach (var item in enumerable)
                {
                    WriteParametersValue(writer, item);
                }

                writer.WriteEndArray();

                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(value), value, null);
        }
    }
}
