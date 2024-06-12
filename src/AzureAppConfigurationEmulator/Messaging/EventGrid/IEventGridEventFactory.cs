using Azure.Messaging.EventGrid;

namespace AzureAppConfigurationEmulator.Messaging.EventGrid;

public interface IEventGridEventFactory
{
    public EventGridEvent Create(string eventType, string dataVersion, BinaryData data);
}
