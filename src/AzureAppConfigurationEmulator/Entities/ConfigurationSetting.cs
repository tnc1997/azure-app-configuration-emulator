namespace AzureAppConfigurationEmulator.Entities;

public class ConfigurationSetting(
    string etag,
    string key,
    string? label,
    string? contentType,
    string? value,
    DateTimeOffset lastModified,
    bool locked,
    IDictionary<string, object?>? tags)
{
    public string Etag { get; set; } = etag;

    public string Key { get; set; } = key;

    public string? Label { get; set; } = label;

    public string? ContentType { get; set; } = contentType;

    public string? Value { get; set; } = value;

    public DateTimeOffset LastModified { get; set; } = lastModified;

    public bool Locked { get; set; } = locked;

    public IDictionary<string, object?>? Tags { get; set; } = tags;
}
