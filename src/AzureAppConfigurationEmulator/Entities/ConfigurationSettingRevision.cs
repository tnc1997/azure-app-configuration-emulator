namespace AzureAppConfigurationEmulator.Entities;

public class ConfigurationSettingRevision(
    string eTag,
    string key,
    string label,
    string? contentType,
    string? value,
    DateTime lastModified,
    bool isReadOnly,
    DateTime validFrom,
    DateTime? validTo)
{
    public ConfigurationSettingRevision(ConfigurationSetting setting) : this(
        setting.ETag,
        setting.Key,
        setting.Label,
        setting.ContentType,
        setting.Value,
        setting.LastModified,
        setting.IsReadOnly,
        setting.LastModified,
        null)
    {
    }

    public string ETag { get; set; } = eTag;

    public string Key { get; set; } = key;

    public string Label { get; set; } = label;

    public string? ContentType { get; set; } = contentType;

    public string? Value { get; set; } = value;

    public DateTime LastModified { get; set; } = lastModified;

    public bool IsReadOnly { get; set; } = isReadOnly;

    public DateTime ValidFrom { get; set; } = validFrom;

    public DateTime? ValidTo { get; set; } = validTo;
}
