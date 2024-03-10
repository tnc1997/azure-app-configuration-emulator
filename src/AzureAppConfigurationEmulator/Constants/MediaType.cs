namespace AzureAppConfigurationEmulator.Constants;

/// <summary>
/// Defines well known media types that are used within Azure App Configuration.
/// </summary>
public static class MediaType
{
    public const string FeatureFlag = "application/vnd.microsoft.appconfig.ff+json";
    
    public const string KeySet = "application/vnd.microsoft.appconfig.keyset+json";
    
    public const string KeyValue = "application/vnd.microsoft.appconfig.kv+json";
    
    public const string KeyValueSet = "application/vnd.microsoft.appconfig.kvset+json";
    
    public const string LabelSet = "application/vnd.microsoft.appconfig.labelset+json";

    public const string SecretReference = "application/vnd.microsoft.appconfig.keyvaultref+json";
}
