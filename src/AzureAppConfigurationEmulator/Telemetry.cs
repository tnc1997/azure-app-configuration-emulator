using System.Diagnostics;

namespace AzureAppConfigurationEmulator;

internal static class Telemetry
{
    public const string ConfigurationSettingContentType = $"{Namespace}.configuration_setting.content_type";
    public const string ConfigurationSettingEtag = $"{Namespace}.configuration_setting.etag";
    public const string ConfigurationSettingKey = $"{Namespace}.configuration_setting.key";
    public const string ConfigurationSettingLabel = $"{Namespace}.configuration_setting.label";
    public const string ConfigurationSettingLastModified = $"{Namespace}.configuration_setting.last_modified";
    public const string ConfigurationSettingLocked = $"{Namespace}.configuration_setting.locked";
    public const string ConfigurationSettingValue = $"{Namespace}.configuration_setting.value";
    public const string DatabaseStatement = "db.statement";
    public const string HeaderAcceptDatetime = $"{Namespace}.header.accept_datetime";
    public const string HeaderIfMatch = $"{Namespace}.header.if_match";
    public const string HeaderIfNoneMatch = $"{Namespace}.header.if_none_match";
    public const string QueryKey = $"{Namespace}.query.key";
    public const string QueryLabel = $"{Namespace}.query.label";
    public const string QueryName = $"{Namespace}.query.name";
    public const string RouteKey = $"{Namespace}.route.key";

    private const string Namespace = "azure_app_configuration_emulator";

    public static ActivitySource ActivitySource { get; } = new(GetName(), GetVersion());

    private static string GetName() => typeof(Program).Assembly.GetName().Name!;

    private static string GetVersion() => typeof(Program).Assembly.GetName().Version!.ToString();
}
