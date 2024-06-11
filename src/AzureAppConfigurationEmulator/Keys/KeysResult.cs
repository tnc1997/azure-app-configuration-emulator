using AzureAppConfigurationEmulator.Common;

namespace AzureAppConfigurationEmulator.Keys;

public class KeysResult(
    IEnumerable<string> keys,
    DateTimeOffset? mementoDatetime = default,
    string? select = default) :
    IResult,
    IContentTypeHttpResult,
    IStatusCodeHttpResult,
    IValueHttpResult
{
    public async Task ExecuteAsync(HttpContext httpContext)
    {
        if (mementoDatetime.HasValue)
        {
            httpContext.Response.Headers["Memento-Datetime"] = mementoDatetime.Value.ToString("R");
        }

        if (StatusCode.HasValue)
        {
            httpContext.Response.StatusCode = StatusCode.Value;
        }

        await httpContext.Response.WriteAsJsonAsync(
            Value,
            new JsonSerializerOptions(JsonSerializerDefaults.Web)
            {
                TypeInfoResolver = new DefaultJsonTypeInfoResolver
                {
                    Modifiers =
                    {
                        new SelectJsonTypeInfoModifier(select?.Split(',')).Modify
                    }
                }
            },
            ContentType);
    }

    public string? ContentType => MediaType.Keys;

    public int? StatusCode => StatusCodes.Status200OK;

    public object Value => new { items = keys.Select(key => new { name = key }) };
}
