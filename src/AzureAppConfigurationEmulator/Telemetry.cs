using System.Diagnostics;

namespace AzureAppConfigurationEmulator;

public static class Telemetry
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
    public const string HeaderContentHash = $"{Namespace}.header.content_hash";
    public const string HeaderDate = $"{Namespace}.header.date";
    public const string HeaderIfMatch = $"{Namespace}.header.if_match";
    public const string HeaderIfNoneMatch = $"{Namespace}.header.if_none_match";
    public const string HmacSignature = $"{Namespace}.hmac.signature";
    public const string HmacSignedHeaders = $"{Namespace}.hmac.signed_headers";
    public const string HmacStringToSign = $"{Namespace}.hmac.string_to_sign";
    public const string MessagingEventId = $"{Namespace}.messaging.event.id";
    public const string MessagingEventSubject = $"{Namespace}.messaging.event.subject";
    public const string MessagingEventTime = $"{Namespace}.messaging.event.time";
    public const string MessagingEventType = $"{Namespace}.messaging.event.type";
    public const string QueryKey = $"{Namespace}.query.key";
    public const string QueryLabel = $"{Namespace}.query.label";
    public const string QueryName = $"{Namespace}.query.name";
    public const string RouteKey = $"{Namespace}.route.key";

    private const string Namespace = "azure_app_configuration_emulator";

    public static ActivitySource ActivitySource { get; } = new("AzureAppConfigurationEmulator");
}
