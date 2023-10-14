namespace AzureAppConfigurationEmulator.Constants;

/// <summary>
/// Defines well known label filters that are used within Azure App Configuration.
/// </summary>
public static class LabelFilter
{
    /// <summary>
    /// The filter that matches key-values with any labels.
    /// </summary>
    public const string Any = "*";

    /// <summary>
    /// The filter that matches key-values with a null label.
    /// </summary>
    public const string Null = "\0";
}
