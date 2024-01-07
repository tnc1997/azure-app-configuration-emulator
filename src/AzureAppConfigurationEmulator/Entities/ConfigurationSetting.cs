namespace AzureAppConfigurationEmulator.Entities;

public class ConfigurationSetting(
    string eTag,
    string key,
    string label,
    string? contentType,
    string? value,
    DateTime lastModified,
    bool isReadOnly)
{
    public ConfigurationSetting(ConfigurationSettingRevision revision) : this(
        revision.ETag,
        revision.Key,
        revision.Label,
        revision.ContentType,
        revision.Value,
        revision.LastModified,
        revision.IsReadOnly)
    {
    }

    public string ETag { get; set; } = eTag;

    public string Key { get; set; } = key;

    public string Label { get; set; } = label;

    public string? ContentType { get; set; } = contentType;

    public string? Value { get; set; } = value;

    public DateTime LastModified { get; set; } = lastModified;

    public bool IsReadOnly { get; set; } = isReadOnly;
}
