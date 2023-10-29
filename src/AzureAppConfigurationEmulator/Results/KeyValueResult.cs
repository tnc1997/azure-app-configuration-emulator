using AzureAppConfigurationEmulator.Entities;
using AzureAppConfigurationEmulator.Extensions;

namespace AzureAppConfigurationEmulator.Results;

public class KeyValueResult(ConfigurationSetting setting) :
    IResult,
    IContentTypeHttpResult,
    IStatusCodeHttpResult,
    IValueHttpResult,
    IValueHttpResult<KeyValue>
{
    public async Task ExecuteAsync(HttpContext httpContext)
    {
        if (StatusCode.HasValue)
        {
            httpContext.Response.StatusCode = StatusCode.Value;
        }

        if (Value is not null)
        {
            await httpContext.Response.WriteAsJsonAsync(Value, options: default, ContentType);
        }
    }

    public string? ContentType => "application/vnd.microsoft.appconfig.kv+json";

    public int? StatusCode => StatusCodes.Status200OK;

    object? IValueHttpResult.Value => Value;

    public KeyValue? Value { get; } = new(
        setting.Key,
        setting.Label.NormalizeNull(),
        setting.ContentType,
        setting.Value,
        setting.LastModified,
        setting.IsReadOnly);
}

public record KeyValue(
    string Key,
    string? Label,
    string? ContentType,
    string? Value,
    DateTimeOffset LastModified,
    bool Locked);