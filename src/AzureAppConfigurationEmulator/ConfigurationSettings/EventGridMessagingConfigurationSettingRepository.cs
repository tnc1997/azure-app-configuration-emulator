using System.Text.Json;
using Azure.Messaging.EventGrid;
using AzureAppConfigurationEmulator.Common;
using AzureAppConfigurationEmulator.Messaging.EventGrid;

namespace AzureAppConfigurationEmulator.ConfigurationSettings;

public class EventGridMessagingConfigurationSettingRepository(
    IConfigurationSettingRepository inner,
    IEventGridEventFactory eventGridEventFactory,
    EventGridPublisherClient eventGridPublisherClient) : IConfigurationSettingRepository
{
    public async Task Add(
        ConfigurationSetting setting,
        CancellationToken cancellationToken = default)
    {
        using var activity = Telemetry.ActivitySource.StartActivity($"{nameof(EventGridMessagingConfigurationSettingRepository)}.{nameof(Add)}");

        await inner.Add(setting, cancellationToken);

        var eventGridEvent = CreateEventGridEvent(EventType.ConfigurationSettingModified, setting);

        activity?.SetTag(Telemetry.MessagingEventId, eventGridEvent.Id);
        activity?.SetTag(Telemetry.MessagingEventSubject, eventGridEvent.Subject);
        activity?.SetTag(Telemetry.MessagingEventTime, eventGridEvent.EventTime);
        activity?.SetTag(Telemetry.MessagingEventType, eventGridEvent.EventType);

        await eventGridPublisherClient.SendEventAsync(eventGridEvent, cancellationToken);
    }

    public IAsyncEnumerable<ConfigurationSetting> Get(
        string key = KeyFilter.Any,
        string label = LabelFilter.Any,
        DateTimeOffset? moment = default,
        CancellationToken cancellationToken = default)
    {
        using var activity = Telemetry.ActivitySource.StartActivity($"{nameof(EventGridMessagingConfigurationSettingRepository)}.{nameof(Get)}");

        return inner.Get(key, label, moment, cancellationToken);
    }

    public async Task Remove(
        ConfigurationSetting setting,
        CancellationToken cancellationToken = default)
    {
        using var activity = Telemetry.ActivitySource.StartActivity($"{nameof(EventGridMessagingConfigurationSettingRepository)}.{nameof(Remove)}");

        await inner.Remove(setting, cancellationToken);

        var eventGridEvent = CreateEventGridEvent(EventType.ConfigurationSettingDeleted, setting);

        activity?.SetTag(Telemetry.MessagingEventId, eventGridEvent.Id);
        activity?.SetTag(Telemetry.MessagingEventSubject, eventGridEvent.Subject);
        activity?.SetTag(Telemetry.MessagingEventTime, eventGridEvent.EventTime);
        activity?.SetTag(Telemetry.MessagingEventType, eventGridEvent.EventType);

        await eventGridPublisherClient.SendEventAsync(eventGridEvent, cancellationToken);
    }

    public async Task Update(
        ConfigurationSetting setting,
        CancellationToken cancellationToken = default)
    {
        using var activity = Telemetry.ActivitySource.StartActivity($"{nameof(EventGridMessagingConfigurationSettingRepository)}.{nameof(Update)}");

        await inner.Update(setting, cancellationToken);

        var eventGridEvent = CreateEventGridEvent(EventType.ConfigurationSettingModified, setting);

        activity?.SetTag(Telemetry.MessagingEventId, eventGridEvent.Id);
        activity?.SetTag(Telemetry.MessagingEventSubject, eventGridEvent.Subject);
        activity?.SetTag(Telemetry.MessagingEventTime, eventGridEvent.EventTime);
        activity?.SetTag(Telemetry.MessagingEventType, eventGridEvent.EventType);

        await eventGridPublisherClient.SendEventAsync(eventGridEvent, cancellationToken);
    }

    private EventGridEvent CreateEventGridEvent(string eventType, ConfigurationSetting setting)
    {
        var options = new JsonSerializerOptions(JsonSerializerDefaults.Web);

        var data = new BinaryData(new { setting.Key, setting.Label, setting.Etag }, options);

        return eventGridEventFactory.Create(eventType, "1", data);
    }
}
