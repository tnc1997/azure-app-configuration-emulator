namespace AzureAppConfigurationEmulator.Results;

public class LabelSetResult(IEnumerable<string?> labels) :
    IResult,
    IContentTypeHttpResult,
    IStatusCodeHttpResult,
    IValueHttpResult,
    IValueHttpResult<LabelSet>
{
    public async Task ExecuteAsync(HttpContext httpContext)
    {
        if (StatusCode.HasValue)
        {
            httpContext.Response.StatusCode = StatusCode.Value;
        }

        await httpContext.Response.WriteAsJsonAsync(Value, options: default, ContentType);
    }

    public string? ContentType => "application/vnd.microsoft.appconfig.labelset+json";

    public int? StatusCode => StatusCodes.Status200OK;

    object IValueHttpResult.Value => Value;

    public LabelSet Value { get; } = new(labels.Select(label => new Label(label)));
}

public record LabelSet(IEnumerable<Label> Items);

public record Label(string? Name);
