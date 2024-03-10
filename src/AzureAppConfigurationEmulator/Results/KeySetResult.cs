using AzureAppConfigurationEmulator.Constants;

namespace AzureAppConfigurationEmulator.Results;

public class KeySetResult(IEnumerable<string> keys, DateTimeOffset? mementoDatetime = default) :
    IResult,
    IContentTypeHttpResult,
    IStatusCodeHttpResult,
    IValueHttpResult,
    IValueHttpResult<KeySet>
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

    public string? ContentType => MediaType.KeySet;

    public int? StatusCode => StatusCodes.Status200OK;

    object IValueHttpResult.Value => Value;

    public KeySet Value { get; } = new(keys.Select(key => new Key(key)));

    private DateTimeOffset? MementoDatetime { get; } = mementoDatetime;
}

public record KeySet(IEnumerable<Key> Items);

public record Key(string Name);
