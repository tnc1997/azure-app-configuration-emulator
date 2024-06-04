using AzureAppConfigurationEmulator.Common;

namespace AzureAppConfigurationEmulator.ConfigurationSettings;

public class ConfigurationSettingsResult(
    IEnumerable<ConfigurationSetting> settings,
    DateTimeOffset? mementoDatetime = default) :
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

        await httpContext.Response.WriteAsJsonAsync(Value, options: default, ContentType);
    }

    public string? ContentType => MediaType.ConfigurationSettings;

    public int? StatusCode => StatusCodes.Status200OK;

    public object Value => new
    {
        items = settings.Select(setting => new
        {
            etag = setting.Etag,
            key = setting.Key,
            label = setting.Label,
            content_type = setting.ContentType,
            value = setting.Value,
            tags = setting.Tags ?? new Dictionary<string, string>(),
            locked = setting.Locked,
            last_modified = setting.LastModified.ToString("O")
        })
    };
}
