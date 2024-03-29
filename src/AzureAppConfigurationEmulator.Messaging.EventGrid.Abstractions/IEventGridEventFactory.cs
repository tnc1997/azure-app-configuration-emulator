using Azure.Messaging.EventGrid;

namespace AzureAppConfigurationEmulator.Messaging.EventGrid.Abstractions;

public interface IEventGridEventFactory
{
    public EventGridEvent Create(string eventType, string dataVersion, BinaryData data);
}
