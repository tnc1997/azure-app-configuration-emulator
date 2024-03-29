using Azure.Messaging.EventGrid;
using AzureAppConfigurationEmulator.Common.Abstractions;
using AzureAppConfigurationEmulator.Common.Constants;
using AzureAppConfigurationEmulator.Common.Models;
using AzureAppConfigurationEmulator.Messaging.EventGrid.Abstractions;
using NSubstitute;
using NUnit.Framework;

namespace AzureAppConfigurationEmulator.ConfigurationSettings.Messaging.EventGrid.Tests;

public class EventGridMessagingConfigurationSettingRepositoryTests
{
    private IEventGridEventFactory EventGridEventFactory { get; set; }

    private EventGridPublisherClient EventGridPublisherClient { get; set; }

    private EventGridMessagingConfigurationSettingRepository Repository { get; set; }

    [SetUp]
    public void SetUp()
    {
        EventGridEventFactory = Substitute.For<IEventGridEventFactory>();
        EventGridEventFactory
            .Create(
                Arg.Any<string>(),
                Arg.Any<string>(),
                Arg.Any<BinaryData>())
            .Returns(info => new EventGridEvent(
                "TestSubject",
                info.ArgAt<string>(0),
                info.ArgAt<string>(1),
                info.ArgAt<BinaryData>(2)));

        EventGridPublisherClient = Substitute.For<EventGridPublisherClient>();

        Repository = new EventGridMessagingConfigurationSettingRepository(
            Substitute.For<IConfigurationSettingRepository>(),
            EventGridEventFactory,
            EventGridPublisherClient);
    }

    [Test]
    public async Task Add_EventGridEventData_ConfigurationSetting()
    {
        // Arrange
        const string etag = "TestEtag";
        const string key = "TestKey";
        const string label = "TestLabel";
        var setting = new ConfigurationSetting(etag, key, DateTimeOffset.UtcNow, false, label);
        
        // Act
        await Repository.Add(setting);
        
        // Assert
        await EventGridPublisherClient
            .Received()
            .SendEventAsync(
                Arg.Is<EventGridEvent>(eventGridEvent =>
                    eventGridEvent.Data.ToString() == new BinaryData(new { key, label, etag }, null, null).ToString()));
    }

    [Test]
    public async Task Add_EventGridEventEventType_ConfigurationSetting()
    {
        // Arrange
        const string etag = "TestEtag";
        const string key = "TestKey";
        const string label = "TestLabel";
        var setting = new ConfigurationSetting(etag, key, DateTimeOffset.UtcNow, false, label);
        
        // Act
        await Repository.Add(setting);
        
        // Assert
        await EventGridPublisherClient
            .Received()
            .SendEventAsync(
                Arg.Is<EventGridEvent>(eventGridEvent =>
                    eventGridEvent.EventType == EventType.ConfigurationSettingModified));
    }

    [Test]
    public async Task Remove_EventGridEventData_ConfigurationSetting()
    {
        // Arrange
        const string etag = "TestEtag";
        const string key = "TestKey";
        const string label = "TestLabel";
        var setting = new ConfigurationSetting(etag, key, DateTimeOffset.UtcNow, false, label);
        
        // Act
        await Repository.Remove(setting);
        
        // Assert
        await EventGridPublisherClient
            .Received()
            .SendEventAsync(
                Arg.Is<EventGridEvent>(eventGridEvent =>
                    eventGridEvent.Data.ToString() == new BinaryData(new { key, label, etag }, null, null).ToString()));
    }

    [Test]
    public async Task Remove_EventGridEventEventType_ConfigurationSetting()
    {
        // Arrange
        const string etag = "TestEtag";
        const string key = "TestKey";
        const string label = "TestLabel";
        var setting = new ConfigurationSetting(etag, key, DateTimeOffset.UtcNow, false, label);
        
        // Act
        await Repository.Remove(setting);
        
        // Assert
        await EventGridPublisherClient
            .Received()
            .SendEventAsync(
                Arg.Is<EventGridEvent>(eventGridEvent =>
                    eventGridEvent.EventType == EventType.ConfigurationSettingDeleted));
    }

    [Test]
    public async Task Update_EventGridEventData_ConfigurationSetting()
    {
        // Arrange
        const string etag = "TestEtag";
        const string key = "TestKey";
        const string label = "TestLabel";
        var setting = new ConfigurationSetting(etag, key, DateTimeOffset.UtcNow, false, label);
        
        // Act
        await Repository.Update(setting);
        
        // Assert
        await EventGridPublisherClient
            .Received()
            .SendEventAsync(
                Arg.Is<EventGridEvent>(eventGridEvent =>
                    eventGridEvent.Data.ToString() == new BinaryData(new { key, label, etag }, null, null).ToString()));
    }

    [Test]
    public async Task Update_EventGridEventEventType_ConfigurationSetting()
    {
        // Arrange
        const string etag = "TestEtag";
        const string key = "TestKey";
        const string label = "TestLabel";
        var setting = new ConfigurationSetting(etag, key, DateTimeOffset.UtcNow, false, label);
        
        // Act
        await Repository.Update(setting);
        
        // Assert
        await EventGridPublisherClient
            .Received()
            .SendEventAsync(
                Arg.Is<EventGridEvent>(eventGridEvent =>
                    eventGridEvent.EventType == EventType.ConfigurationSettingModified));
    }
}
