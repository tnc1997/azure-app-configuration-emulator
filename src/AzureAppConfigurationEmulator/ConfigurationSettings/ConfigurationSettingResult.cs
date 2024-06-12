using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using AzureAppConfigurationEmulator.Common;

namespace AzureAppConfigurationEmulator.ConfigurationSettings;

public class ConfigurationSettingResult(
    ConfigurationSetting setting,
    DateTimeOffset? mementoDatetime = default,
    string? select = default) :
    IResult,
    IContentTypeHttpResult,
    IStatusCodeHttpResult,
    IValueHttpResult
{
    public async Task ExecuteAsync(HttpContext httpContext)
    {
        httpContext.Response.Headers.ETag = setting.Etag;
        httpContext.Response.Headers.LastModified = setting.LastModified.ToString("R");

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

    public string? ContentType => MediaType.ConfigurationSetting;

    public int? StatusCode => StatusCodes.Status200OK;

    public object Value => new
    {
        etag = setting.Etag,
        key = setting.Key,
        label = setting.Label,
        content_type = setting.ContentType,
        value = setting.Value,
        tags = setting.Tags ?? new Dictionary<string, string>(),
        locked = setting.Locked,
        last_modified = setting.LastModified.ToString("O")
    };
}
