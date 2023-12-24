namespace AzureAppConfigurationEmulator.Entities;

public class ConfigurationSettingRevision(
    string eTag,
    string key,
    string label,
    string? contentType,
    string? value,
    DateTimeOffset lastModified,
    bool isReadOnly,
    DateTimeOffset validFrom,
    DateTimeOffset? validTo)
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

    public DateTimeOffset LastModified { get; set; } = lastModified;

    public bool IsReadOnly { get; set; } = isReadOnly;

    public DateTimeOffset ValidFrom { get; set; } = validFrom;

    public DateTimeOffset? ValidTo { get; set; } = validTo;
}
