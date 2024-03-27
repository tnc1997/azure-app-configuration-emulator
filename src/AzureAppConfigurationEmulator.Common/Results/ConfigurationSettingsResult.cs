using AzureAppConfigurationEmulator.Common.Constants;
using AzureAppConfigurationEmulator.Common.Models;
using Microsoft.AspNetCore.Http;

namespace AzureAppConfigurationEmulator.Common.Results;

public class ConfigurationSettingsResult(IEnumerable<ConfigurationSetting> settings, DateTimeOffset? mementoDatetime = default) :
    IResult,
    IContentTypeHttpResult,
    IStatusCodeHttpResult,
    IValueHttpResult,
    IValueHttpResult<ConfigurationSettings>
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

    public string? ContentType => MediaType.ConfigurationSettings;

    public int? StatusCode => StatusCodes.Status200OK;

    object IValueHttpResult.Value => Value;

    public ConfigurationSettings Value { get; } = new(settings);

    private DateTimeOffset? MementoDatetime { get; } = mementoDatetime;
}

public record ConfigurationSettings(IEnumerable<ConfigurationSetting> Items);
