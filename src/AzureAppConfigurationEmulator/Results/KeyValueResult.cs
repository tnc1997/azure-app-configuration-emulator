using AzureAppConfigurationEmulator.Entities;

namespace AzureAppConfigurationEmulator.Results;

public class KeyValueResult(ConfigurationSetting setting) :
    IResult,
    IContentTypeHttpResult,
    IStatusCodeHttpResult,
    IValueHttpResult,
    IValueHttpResult<ConfigurationSetting>
{
    public async Task ExecuteAsync(HttpContext httpContext)
    {
        httpContext.Response.Headers.ETag = Value.Etag;
        httpContext.Response.Headers.LastModified = Value.LastModified.ToString("R");

        if (StatusCode.HasValue)
        {
            httpContext.Response.StatusCode = StatusCode.Value;
        }

        await httpContext.Response.WriteAsJsonAsync(Value, options: default, ContentType);
    }

    public string? ContentType => "application/vnd.microsoft.appconfig.kv+json";

    public int? StatusCode => StatusCodes.Status200OK;

    object IValueHttpResult.Value => Value;

    public ConfigurationSetting Value { get; } = setting;
}
