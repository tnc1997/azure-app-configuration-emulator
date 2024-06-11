using AzureAppConfigurationEmulator.Messaging.EventGrid;
using Microsoft.AspNetCore.Http;
using NSubstitute;
using NUnit.Framework;

namespace AzureAppConfigurationEmulator.Tests.Messaging.EventGrid;

public class HttpContextEventGridEventFactoryTests
{
    private HttpContextEventGridEventFactory EventGridEventFactory { get; set; }

    private IHttpContextAccessor HttpContextAccessor { get; set; }

    [SetUp]
    public void SetUp()
    {
        HttpContextAccessor = Substitute.For<IHttpContextAccessor>();

        EventGridEventFactory = new HttpContextEventGridEventFactory(HttpContextAccessor);
    }

    [Test]
    public void Create_EventGridEventData_Data()
    {
        // Arrange
        var data = new BinaryData(new { key = "TestKey", label = "TestLabel", etag = "TestEtag" });

        // Act
        var eventGridEvent = EventGridEventFactory.Create("TestEventType", "TestDataVersion", data);

        // Assert
        Assert.That(eventGridEvent.Data.ToString(), Is.EqualTo(data.ToString()));
    }

    [Test]
    public void Create_EventGridEventDataVersion_DataVersion()
    {
        // Act
        var eventGridEvent = EventGridEventFactory.Create("TestEventType", "TestDataVersion", new BinaryData(""));

        // Assert
        Assert.That(eventGridEvent.DataVersion, Is.EqualTo("TestDataVersion"));
    }

    [Test]
    public void Create_EventGridEventEventType_EventType()
    {
        // Act
        var eventGridEvent = EventGridEventFactory.Create("TestEventType", "TestDataVersion", new BinaryData(""));

        // Assert
        Assert.That(eventGridEvent.EventType, Is.EqualTo("TestEventType"));
    }

    [Test]
    public void Create_EventGridEventSubject_Subject()
    {
        // Arrange
        HttpContextAccessor.HttpContext.Returns(new DefaultHttpContext
        {
            Request =
            {
                Scheme = "https",
                Host = new HostString("contoso.azconfig.io"),
                Path = new PathString("/kv/TestKey"),
                QueryString = new QueryString("?label=TestLabel")
            }
        });

        // Act
        var eventGridEvent = EventGridEventFactory.Create("TestEventType", "TestDataVersion", new BinaryData(""));

        // Assert
        Assert.That(eventGridEvent.Subject, Is.EqualTo("https://contoso.azconfig.io/kv/TestKey?label=TestLabel"));
    }
}
