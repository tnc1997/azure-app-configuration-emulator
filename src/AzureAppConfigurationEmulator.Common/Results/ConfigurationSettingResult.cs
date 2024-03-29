using AzureAppConfigurationEmulator.Common.Constants;
using AzureAppConfigurationEmulator.Common.Models;
using Microsoft.AspNetCore.Http;

namespace AzureAppConfigurationEmulator.Common.Results;

public class ConfigurationSettingResult(ConfigurationSetting setting, DateTimeOffset? mementoDatetime = default) :
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

    public string? ContentType => MediaType.ConfigurationSetting;

    public int? StatusCode => StatusCodes.Status200OK;

    object IValueHttpResult.Value => Value;

    public ConfigurationSetting Value { get; } = setting;

    private DateTimeOffset? MementoDatetime { get; } = mementoDatetime;
}
