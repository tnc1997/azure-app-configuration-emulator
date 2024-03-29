using Azure.Messaging.EventGrid;
using AzureAppConfigurationEmulator.Messaging.EventGrid.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;

namespace AzureAppConfigurationEmulator.Messaging.EventGrid.HttpContext;

public class HttpContextEventGridEventFactory(IHttpContextAccessor httpContextAccessor) : IEventGridEventFactory
{
    public EventGridEvent Create(string eventType, string dataVersion, BinaryData data)
    {
        var subject = httpContextAccessor.HttpContext?.Request.GetDisplayUrl() ?? throw new InvalidOperationException();

        return new EventGridEvent(subject, eventType, dataVersion, data);
    }
}
