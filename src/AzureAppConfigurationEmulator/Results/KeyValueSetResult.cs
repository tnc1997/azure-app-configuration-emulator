using AzureAppConfigurationEmulator.Entities;

namespace AzureAppConfigurationEmulator.Results;

public class KeyValueSetResult(IEnumerable<ConfigurationSetting> settings) :
    IResult,
    IContentTypeHttpResult,
    IStatusCodeHttpResult,
    IValueHttpResult,
    IValueHttpResult<KeyValueSet>
{
    public async Task ExecuteAsync(HttpContext httpContext)
    {
        if (StatusCode.HasValue)
        {
            httpContext.Response.StatusCode = StatusCode.Value;
        }

        await httpContext.Response.WriteAsJsonAsync(Value, options: default, ContentType);
    }

    public string? ContentType => "application/vnd.microsoft.appconfig.kvset+json";

    public int? StatusCode => StatusCodes.Status200OK;

    object IValueHttpResult.Value => Value;

    public KeyValueSet Value { get; } = new(settings);
}

public record KeyValueSet(IEnumerable<ConfigurationSetting> Items);
