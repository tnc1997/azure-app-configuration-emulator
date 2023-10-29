using Microsoft.AspNetCore.Mvc;

namespace AzureAppConfigurationEmulator.Results;

public class InvalidCharacterResult(string name) :
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

    public int? StatusCode => StatusCodes.Status400BadRequest;

    object? IValueHttpResult.Value => Value;

    public ProblemDetails? Value => new()
    {
        Type = "https://azconfig.io/errors/invalid-argument",
        Title = $"Invalid request parameter '{name}'",
        Status = StatusCode,
        Detail = $"{name}: Invalid character",
        Extensions = new Dictionary<string, object?> { { nameof(name), name } }
    };
}
