using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Metadata;

namespace AzureAppConfigurationEmulator.Common.Results;

public class NotModifiedResult :
    IResult,
    IEndpointMetadataProvider,
    IStatusCodeHttpResult
{
    public Task ExecuteAsync(HttpContext httpContext)
    {
        if (StatusCode.HasValue)
        {
            httpContext.Response.StatusCode = StatusCode.Value;
        }

        return Task.CompletedTask;
    }

    public static void PopulateMetadata(MethodInfo method, EndpointBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(method);
        ArgumentNullException.ThrowIfNull(builder);

        builder.Metadata.Add(new ProducesResponseTypeMetadata(StatusCodes.Status304NotModified, typeof(void)));
    }

    public int? StatusCode => StatusCodes.Status304NotModified;
}
