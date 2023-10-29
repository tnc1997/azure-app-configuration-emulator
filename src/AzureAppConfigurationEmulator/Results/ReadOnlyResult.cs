using Microsoft.AspNetCore.Mvc;

namespace AzureAppConfigurationEmulator.Results;

public class ReadOnlyResult(string key) :
    IResult,
    IContentTypeHttpResult,
    IStatusCodeHttpResult,
    IValueHttpResult,
    IValueHttpResult<ProblemDetails>
{
    public async Task ExecuteAsync(HttpContext httpContext)
    {
        if (StatusCode.HasValue)
        {
            httpContext.Response.StatusCode = StatusCode.Value;
        }

        if (Value is not null)
        {
            await httpContext.Response.WriteAsJsonAsync(Value, options: default, ContentType);
        }
    }

    public string? ContentType => "application/problem+json";

    public int? StatusCode => StatusCodes.Status409Conflict;

    object? IValueHttpResult.Value => Value;

    public ProblemDetails? Value => new()
    {
        Type = "https://azconfig.io/errors/key-locked",
        Title = $"Modifying key '{key}' is not allowed",
        Status = StatusCode,
        Detail = "The key is read-only. To allow modification unlock it first.",
        Extensions = new Dictionary<string, object?> { { "name", key } }
    };
}
