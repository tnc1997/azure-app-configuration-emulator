using System.Diagnostics;

namespace AzureAppConfigurationEmulator.Authentication.Hmac;

public static class Telemetry
{
    public const string ContentHash = $"{Namespace}.content_hash";
    public const string Date = $"{Namespace}.date";
    public const string Signature = $"{Namespace}.signature";
    public const string StringToSign = $"{Namespace}.string_to_sign";

    private const string Namespace = "azure_app_configuration_emulator.authentication.hmac";

    public static ActivitySource ActivitySource { get; } = new("AzureAppConfigurationEmulator.Authentication.Hmac");
}
