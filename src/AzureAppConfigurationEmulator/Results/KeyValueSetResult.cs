using AzureAppConfigurationEmulator.Constants;
using AzureAppConfigurationEmulator.Entities;

namespace AzureAppConfigurationEmulator.Results;

public class KeyValueSetResult(IEnumerable<ConfigurationSetting> settings, DateTimeOffset? mementoDatetime = default) :
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

        await httpContext.Response.WriteAsJsonAsync(Value, options: default, ContentType);
    }

    public string? ContentType => MediaType.KeyValueSet;

    public int? StatusCode => StatusCodes.Status200OK;

    object IValueHttpResult.Value => Value;

    public KeyValueSet Value { get; } = new(settings);

    private DateTimeOffset? MementoDatetime { get; } = mementoDatetime;
}

public record KeyValueSet(IEnumerable<ConfigurationSetting> Items);
