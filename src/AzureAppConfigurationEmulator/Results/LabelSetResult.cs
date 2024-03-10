using AzureAppConfigurationEmulator.Constants;

namespace AzureAppConfigurationEmulator.Results;

public class LabelSetResult(IEnumerable<string?> labels, DateTimeOffset? mementoDatetime = default) :
    IResult,
    IContentTypeHttpResult,
    IStatusCodeHttpResult,
    IValueHttpResult,
    IValueHttpResult<LabelSet>
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

    public string? ContentType => MediaType.LabelSet;

    public int? StatusCode => StatusCodes.Status200OK;

    object IValueHttpResult.Value => Value;

    public LabelSet Value { get; } = new(labels.Select(label => new Label(label)));

    private DateTimeOffset? MementoDatetime { get; } = mementoDatetime;
}

public record LabelSet(IEnumerable<Label> Items);

public record Label(string? Name);
