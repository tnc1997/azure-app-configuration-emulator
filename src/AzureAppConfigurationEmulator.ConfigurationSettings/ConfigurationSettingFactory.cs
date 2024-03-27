using System.Net.Mime;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using AzureAppConfigurationEmulator.Common.Abstractions;
using AzureAppConfigurationEmulator.Common.Constants;
using AzureAppConfigurationEmulator.Common.Models;

namespace AzureAppConfigurationEmulator.ConfigurationSettings;

public class ConfigurationSettingFactory : IConfigurationSettingFactory
{
    public ConfigurationSetting Create(
        string key,
        string? label = null,
        string? contentType = null,
        string? value = null,
        IReadOnlyDictionary<string, object?>? tags = null)
    {
        var date = DateTimeOffset.UtcNow;

        return Create(
            Convert.ToBase64String(
                SHA256.HashData(
                    Encoding.UTF8.GetBytes(
                        date.UtcDateTime.ToString(
                            "yyyy-MM-dd HH:mm:ss")))),
            key,
            date,
            false,
            label,
            contentType,
            value,
            tags);
    }

    public ConfigurationSetting Create(
        string etag,
        string key,
        DateTimeOffset lastModified,
        bool locked,
        string? label = null,
        string? contentType = null,
        string? value = null,
        IReadOnlyDictionary<string, object?>? tags = null)
    {
        if (!string.IsNullOrEmpty(contentType) && !string.IsNullOrEmpty(value))
        {
            switch (new ContentType(contentType).MediaType)
            {
                case MediaType.FeatureFlag:
                {
                    using var document = JsonDocument.Parse(value);

                    return new FeatureFlagConfigurationSetting(
                        document.RootElement.GetProperty("id").GetString()!,
                        document.RootElement.GetProperty("enabled").GetBoolean(),
                        document.RootElement.TryGetProperty("conditions", out var conditions) && conditions.TryGetProperty("client_filters", out var clientFilters)
                            ? clientFilters.EnumerateArray()
                                .Select(clientFilter =>
                                    new FeatureFlagFilter(
                                        clientFilter.GetProperty("name").GetString()!,
                                        clientFilter.TryGetProperty("properties", out var properties)
                                            ? properties.Deserialize<IReadOnlyDictionary<string, object?>>()
                                            : null))
                                .ToList()
                            : [],
                        etag,
                        key,
                        value,
                        lastModified,
                        locked,
                        document.RootElement.TryGetProperty("description", out var description)
                            ? description.GetString()
                            : null,
                        document.RootElement.TryGetProperty("display_name", out var displayName)
                            ? displayName.GetString()
                            : null,
                        label,
                        contentType,
                        tags);
                }
            }
        }

        return new ConfigurationSetting(
            etag,
            key,
            lastModified,
            locked,
            label,
            contentType,
            value,
            tags);
    }
}
