namespace AzureAppConfigurationEmulator.Common.Models;

/// <summary>
/// https://github.com/Azure/azure-sdk-for-net/blob/main/sdk/appconfiguration/Azure.Data.AppConfiguration/src/Models/ConfigurationSetting.cs
/// </summary>
public class ConfigurationSetting(
    string etag,
    string key,
    DateTimeOffset lastModified,
    bool locked,
    string? label = null,
    string? contentType = null,
    string? value = null,
    IDictionary<string, string>? tags = null)
{
    public string Etag { get; set; } = etag;

    public string Key { get; set; } = key;

    public string? Label { get; set; } = label;

    public string? ContentType { get; set; } = contentType;

    public virtual string? Value { get; set; } = value;

    public DateTimeOffset LastModified { get; set; } = lastModified;

    public bool Locked { get; set; } = locked;

    public IDictionary<string, string>? Tags { get; set; } = tags;
}
