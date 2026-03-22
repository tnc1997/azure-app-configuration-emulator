using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using AzureAppConfigurationEmulator.Common;

namespace AzureAppConfigurationEmulator.Labels;

public class LabelsResult(
    IEnumerable<string?> labels,
    DateTimeOffset? mementoDatetime = null,
    string? select = null) :
    IResult,
    IContentTypeHttpResult,
    IStatusCodeHttpResult,
    IValueHttpResult
{
    public async Task ExecuteAsync(HttpContext httpContext)
    {
        if (mementoDatetime is not null)
        {
            httpContext.Response.Headers["Memento-Datetime"] = mementoDatetime.Value.ToString("R");
        }

        if (StatusCode is not null)
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

    public string? ContentType => MediaType.Labels;

    public int? StatusCode => StatusCodes.Status200OK;

    public object Value => new { items = labels.Select(label => new { name = label }) };
}
