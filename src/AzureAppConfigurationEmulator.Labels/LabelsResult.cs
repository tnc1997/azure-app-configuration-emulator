using AzureAppConfigurationEmulator.Common.Constants;
using Microsoft.AspNetCore.Http;

namespace AzureAppConfigurationEmulator.Labels;

public class LabelsResult(
    IEnumerable<string?> labels,
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

    public string? ContentType => MediaType.Labels;

    public int? StatusCode => StatusCodes.Status200OK;

    public object Value => new { items = labels.Select(label => new { name = label }) };
}
