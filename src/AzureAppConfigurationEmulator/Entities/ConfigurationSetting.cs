namespace AzureAppConfigurationEmulator.Entities;

public class ConfigurationSetting(
    string eTag,
    string key,
    string label,
    string? contentType,
    string? value,
    DateTimeOffset lastModified,
    bool isReadOnly)
{
    public string ETag { get; set; } = eTag;

    public string Key { get; set; } = key;

    public string Label { get; set; } = label;

    public string? ContentType { get; set; } = contentType;

    public string? Value { get; set; } = value;

    public DateTimeOffset LastModified { get; set; } = lastModified;

    public bool IsReadOnly { get; set; } = isReadOnly;
}
