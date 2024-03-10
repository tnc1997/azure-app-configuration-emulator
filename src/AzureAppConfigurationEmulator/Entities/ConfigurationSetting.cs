namespace AzureAppConfigurationEmulator.Entities;

/// <summary>
/// https://github.com/Azure/azure-sdk-for-net/blob/main/sdk/appconfiguration/Azure.Data.AppConfiguration/src/Models/ConfigurationSetting.cs
/// </summary>
public record ConfigurationSetting(
    string Etag,
    string Key,
    DateTimeOffset LastModified,
    bool Locked,
    string? Label = null,
    string? ContentType = null,
    string? Value = null,
    IReadOnlyDictionary<string, object?>? Tags = null);

/// <summary>
/// https://github.com/Azure/azure-sdk-for-net/blob/main/sdk/appconfiguration/Azure.Data.AppConfiguration/src/Models/FeatureFlagConfigurationSetting.cs
/// </summary>
public record FeatureFlagConfigurationSetting(
    string Id,
    bool Enabled,
    IReadOnlyCollection<FeatureFlagFilter> ClientFilters,
    string Etag,
    string Key,
    string Value,
    DateTimeOffset LastModified,
    bool Locked,
    string? Description = null,
    string? DisplayName = null,
    string? Label = null,
    string? ContentType = null,
    IReadOnlyDictionary<string, object?>? Tags = null)
    : ConfigurationSetting(
        Etag,
        Key,
        LastModified,
        Locked,
        Label,
        ContentType,
        Value,
        Tags);

/// <summary>
/// https://github.com/Azure/azure-sdk-for-net/blob/main/sdk/appconfiguration/Azure.Data.AppConfiguration/src/Models/FeatureFlagFilter.cs
/// </summary>
public record FeatureFlagFilter(
    string Name,
    IReadOnlyDictionary<string, object?>? Parameters);
