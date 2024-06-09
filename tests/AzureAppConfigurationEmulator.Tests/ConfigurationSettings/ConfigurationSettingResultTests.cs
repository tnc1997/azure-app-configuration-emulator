using AzureAppConfigurationEmulator.Common;
using AzureAppConfigurationEmulator.ConfigurationSettings;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using NUnit.Framework;

namespace AzureAppConfigurationEmulator.Tests.ConfigurationSettings;

public class ConfigurationSettingResultTests
{
    private HttpContext HttpContext { get; set; }

    [SetUp]
    public void SetUp()
    {
        HttpContext = new DefaultHttpContext();
    }

    [Test]
    public async Task ExecuteAsync_ContentTypeResponseHeader_ConfigurationSetting()
    {
        // Arrange
        var setting = new ConfigurationSetting("TestEtag", "TestKey", DateTimeOffset.UtcNow, false);

        // Act
        await new ConfigurationSettingResult(setting).ExecuteAsync(HttpContext);

        // Assert
        Assert.That(HttpContext.Response.Headers.ContentType, Is.EqualTo(MediaType.ConfigurationSetting));
    }

    [Test]
    public async Task ExecuteAsync_EtagResponseHeader_ConfigurationSetting()
    {
        // Arrange
        const string etag = "TestEtag";
        var setting = new ConfigurationSetting(etag, "TestKey", DateTimeOffset.UtcNow, false);

        // Act
        await new ConfigurationSettingResult(setting).ExecuteAsync(HttpContext);

        // Assert
        Assert.That(HttpContext.Response.Headers.ETag, Is.EqualTo(etag));
    }

    [Test]
    public async Task ExecuteAsync_LastModifiedResponseHeader_ConfigurationSetting()
    {
        // Arrange
        var lastModified = DateTimeOffset.UtcNow;
        var setting = new ConfigurationSetting("TestEtag", "TestKey", lastModified, false);

        // Act
        await new ConfigurationSettingResult(setting).ExecuteAsync(HttpContext);

        // Assert
        Assert.That(HttpContext.Response.Headers.LastModified, Is.EqualTo(lastModified.ToString("R")));
    }

    [Test]
    public async Task ExecuteAsync_MementoDatetimeResponseHeader_MementoDatetime()
    {
        // Arrange
        var setting = new ConfigurationSetting("TestEtag", "TestKey", DateTimeOffset.UtcNow, false);
        var mementoDatetime = DateTimeOffset.UtcNow;

        // Act
        await new ConfigurationSettingResult(setting, mementoDatetime).ExecuteAsync(HttpContext);

        // Assert
        Assert.That(HttpContext.Response.Headers["Memento-Datetime"], Is.EqualTo(mementoDatetime.ToString("R")));
    }

    [TestCaseSource(nameof(ExecuteAsync_ResponseBody_ConfigurationSetting_TestCases))]
    public async Task ExecuteAsync_ResponseBody_ConfigurationSetting(ConfigurationSetting setting, string expected)
    {
        // Arrange
        using var stream = new MemoryStream();
        var feature = new StreamResponseBodyFeature(stream);
        HttpContext.Features.Set<IHttpResponseBodyFeature>(feature);

        // Act
        await new ConfigurationSettingResult(setting).ExecuteAsync(HttpContext);

        // Assert
        stream.Seek(0, SeekOrigin.Begin);
        using var reader = new StreamReader(stream);
        var actual = await reader.ReadToEndAsync();
        Assert.That(actual, Is.EqualTo(expected));
    }

    // ReSharper disable once InconsistentNaming
    private static object[] ExecuteAsync_ResponseBody_ConfigurationSetting_TestCases =
    [
        new object?[]
        {
            new ConfigurationSetting(
                "TestEtag",
                "TestKey",
                DateTimeOffset.Parse("2023-10-01T00:00:00+00:00"),
                false),
            "{\"etag\":\"TestEtag\",\"key\":\"TestKey\",\"label\":null,\"content_type\":null,\"value\":null,\"tags\":{},\"locked\":false,\"last_modified\":\"2023-10-01T00:00:00.0000000\\u002B00:00\"}"
        },
        new object?[]
        {
            new ConfigurationSetting(
                "TestEtag",
                "TestKey",
                DateTimeOffset.Parse("2023-10-01T00:00:00+00:00"),
                false,
                label: "TestLabel"),
            "{\"etag\":\"TestEtag\",\"key\":\"TestKey\",\"label\":\"TestLabel\",\"content_type\":null,\"value\":null,\"tags\":{},\"locked\":false,\"last_modified\":\"2023-10-01T00:00:00.0000000\\u002B00:00\"}"
        },
        new object?[]
        {
            new ConfigurationSetting(
                "TestEtag",
                "TestKey",
                DateTimeOffset.Parse("2023-10-01T00:00:00+00:00"),
                false,
                contentType: "TestContentType"),
            "{\"etag\":\"TestEtag\",\"key\":\"TestKey\",\"label\":null,\"content_type\":\"TestContentType\",\"value\":null,\"tags\":{},\"locked\":false,\"last_modified\":\"2023-10-01T00:00:00.0000000\\u002B00:00\"}"
        },
        new object?[]
        {
            new ConfigurationSetting(
                "TestEtag",
                "TestKey",
                DateTimeOffset.Parse("2023-10-01T00:00:00+00:00"),
                false,
                value: "TestValue"),
            "{\"etag\":\"TestEtag\",\"key\":\"TestKey\",\"label\":null,\"content_type\":null,\"value\":\"TestValue\",\"tags\":{},\"locked\":false,\"last_modified\":\"2023-10-01T00:00:00.0000000\\u002B00:00\"}"
        },
        new object?[]
        {
            new ConfigurationSetting(
                "TestEtag",
                "TestKey",
                DateTimeOffset.Parse("2023-10-01T00:00:00+00:00"),
                false,
                tags: new Dictionary<string, string> { { "TestKey", "TestValue" } }),
            "{\"etag\":\"TestEtag\",\"key\":\"TestKey\",\"label\":null,\"content_type\":null,\"value\":null,\"tags\":{\"TestKey\":\"TestValue\"},\"locked\":false,\"last_modified\":\"2023-10-01T00:00:00.0000000\\u002B00:00\"}"
        },
        new object?[]
        {
            new ConfigurationSetting(
                "TestEtag",
                "TestKey",
                DateTimeOffset.Parse("2023-10-01T00:00:00+00:00"),
                false,
                "TestLabel",
                "TestContentType",
                "TestValue",
                new Dictionary<string, string> { { "TestKey", "TestValue" } }),
            "{\"etag\":\"TestEtag\",\"key\":\"TestKey\",\"label\":\"TestLabel\",\"content_type\":\"TestContentType\",\"value\":\"TestValue\",\"tags\":{\"TestKey\":\"TestValue\"},\"locked\":false,\"last_modified\":\"2023-10-01T00:00:00.0000000\\u002B00:00\"}"
        }
    ];

    [Test]
    public async Task ExecuteAsync_ResponseStatusCode_ConfigurationSetting()
    {
        // Arrange
        var setting = new ConfigurationSetting("TestEtag", "TestKey", DateTimeOffset.UtcNow, false);

        // Act
        await new ConfigurationSettingResult(setting).ExecuteAsync(HttpContext);

        // Assert
        Assert.That(HttpContext.Response.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
    }
}
