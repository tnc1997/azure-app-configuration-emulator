using System.Net.Mime;
using System.Security.Cryptography;
using System.Text;
using AzureAppConfigurationEmulator.Common;
using OpenTelemetry.Trace;

namespace AzureAppConfigurationEmulator.ConfigurationSettings;

public class ConfigurationSettingFactory : IConfigurationSettingFactory
{
    public ConfigurationSetting Create(
        string key,
        string? label = null,
        string? contentType = null,
        string? value = null,
        IDictionary<string, string>? tags = null)
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
        IDictionary<string, string>? tags = null)
    {
        using var activity = Telemetry.ActivitySource.StartActivity($"{nameof(ConfigurationSettingFactory)}.{nameof(Create)}");
        activity?.SetTag(Telemetry.ConfigurationSettingEtag, etag);
        activity?.SetTag(Telemetry.ConfigurationSettingKey, key);
        activity?.SetTag(Telemetry.ConfigurationSettingLabel, label);
        activity?.SetTag(Telemetry.ConfigurationSettingContentType, contentType);
        activity?.SetTag(Telemetry.ConfigurationSettingValue, value);
        activity?.SetTag(Telemetry.ConfigurationSettingLastModified, lastModified);
        activity?.SetTag(Telemetry.ConfigurationSettingLocked, locked);

        if (!string.IsNullOrEmpty(contentType) && !string.IsNullOrEmpty(value))
        {
            try
            {
                if (new ContentType(contentType).MediaType is MediaType.FeatureFlag)
                {
                    return new FeatureFlagConfigurationSetting(
                        etag,
                        key,
                        value,
                        lastModified,
                        locked,
                        label,
                        contentType,
                        tags);
                }
            }
            catch (Exception e)
            {
                activity?.RecordException(e);
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
