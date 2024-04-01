namespace AzureAppConfigurationEmulator.Common.Constants;

/// <summary>
/// Defines well known media types that are used within Azure App Configuration.
/// </summary>
public static class MediaType
{
    public const string ConfigurationSetting = "application/vnd.microsoft.appconfig.kv+json";

    public const string ConfigurationSettings = "application/vnd.microsoft.appconfig.kvset+json";

    public const string FeatureFlag = "application/vnd.microsoft.appconfig.ff+json";

    public const string Json = "application/json";

    public const string Keys = "application/vnd.microsoft.appconfig.keyset+json";

    public const string Labels = "application/vnd.microsoft.appconfig.labelset+json";

    public const string SecretReference = "application/vnd.microsoft.appconfig.keyvaultref+json";
}
