namespace AzureAppConfigurationEmulator.Results;

public class KeySetResult(IEnumerable<string> keys) :
    IResult,
    IContentTypeHttpResult,
    IStatusCodeHttpResult,
    IValueHttpResult,
    IValueHttpResult<KeySet>
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

    public string? ContentType => "application/vnd.microsoft.appconfig.keyset+json";

    public int? StatusCode => StatusCodes.Status200OK;

    object? IValueHttpResult.Value => Value;

    public KeySet? Value { get; } = new(keys.Select(key => new Key(key)));
}

public record KeySet(IEnumerable<Key> Items);

public record Key(string Name);
