using AzureAppConfigurationEmulator.Entities;
using AzureAppConfigurationEmulator.Extensions;

namespace AzureAppConfigurationEmulator.Results;

public class KeyValueSetResult(IEnumerable<ConfigurationSetting> settings, DateTime? mementoDatetime = default) :
    IResult,
    IContentTypeHttpResult,
    IStatusCodeHttpResult,
    IValueHttpResult,
    IValueHttpResult<KeyValueSet>
{
    public async Task ExecuteAsync(HttpContext httpContext)
    {
        if (MementoDatetime.HasValue)
        {
            httpContext.Response.Headers["Memento-Datetime"] = MementoDatetime.Value.ToString("R");
        }

        if (StatusCode.HasValue)
        {
            httpContext.Response.StatusCode = StatusCode.Value;
        }

        if (Value is not null)
        {
            await httpContext.Response.WriteAsJsonAsync(Value, options: default, ContentType);
        }
    }

    public string? ContentType => "application/vnd.microsoft.appconfig.kvset+json";

    public int? StatusCode => StatusCodes.Status200OK;

    object? IValueHttpResult.Value => Value;

    public KeyValueSet? Value { get; } = new(
        settings.Select(
            setting => new KeyValue(
                setting.ETag,
                setting.Key,
                setting.Label.NormalizeNull(),
                setting.ContentType,
                setting.Value,
                setting.LastModified,
                setting.IsReadOnly)));

    private DateTime? MementoDatetime { get; } = mementoDatetime;
}

public record KeyValueSet(IEnumerable<KeyValue> Items);
