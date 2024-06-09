using AzureAppConfigurationEmulator.Common;
using AzureAppConfigurationEmulator.ConfigurationSettings;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using NUnit.Framework;

namespace AzureAppConfigurationEmulator.Tests.ConfigurationSettings;

public class ConfigurationSettingsResultTests
{
    private HttpContext HttpContext { get; set; }

    [SetUp]
    public void SetUp()
    {
        HttpContext = new DefaultHttpContext();
    }

    [Test]
    public async Task ExecuteAsync_ContentTypeResponseHeader_ConfigurationSettings()
    {
        // Arrange
        var settings = new List<ConfigurationSetting> { new("TestEtag", "TestKey", DateTimeOffset.UtcNow, false) };

        // Act
        await new ConfigurationSettingsResult(settings).ExecuteAsync(HttpContext);

        // Assert
        Assert.That(HttpContext.Response.Headers.ContentType, Is.EqualTo(MediaType.ConfigurationSettings));
    }

    [Test]
    public async Task ExecuteAsync_MementoDatetimeResponseHeader_MementoDatetime()
    {
        // Arrange
        var settings = new List<ConfigurationSetting> { new("TestEtag", "TestKey", DateTimeOffset.UtcNow, false) };
        var mementoDatetime = DateTimeOffset.UtcNow;

        // Act
        await new ConfigurationSettingsResult(settings, mementoDatetime).ExecuteAsync(HttpContext);

        // Assert
        Assert.That(HttpContext.Response.Headers["Memento-Datetime"], Is.EqualTo(mementoDatetime.ToString("R")));
    }

    [TestCaseSource(nameof(ExecuteAsync_ResponseBody_ConfigurationSettings_TestCases))]
    public async Task ExecuteAsync_ResponseBody_ConfigurationSettings(ConfigurationSetting[] settings, string expected)
    {
        // Arrange
        using var stream = new MemoryStream();
        var feature = new StreamResponseBodyFeature(stream);
        HttpContext.Features.Set<IHttpResponseBodyFeature>(feature);

        // Act
        await new ConfigurationSettingsResult(settings).ExecuteAsync(HttpContext);

        // Assert
        stream.Seek(0, SeekOrigin.Begin);
        using var reader = new StreamReader(stream);
        var actual = await reader.ReadToEndAsync();
        Assert.That(actual, Is.EqualTo(expected));
    }

    // ReSharper disable once InconsistentNaming
    private static object[] ExecuteAsync_ResponseBody_ConfigurationSettings_TestCases =
    [
        new object?[]
        {
            new ConfigurationSetting[]
            {
                new(
                    "TestEtag",
                    "TestKey",
                    DateTimeOffset.Parse("2023-10-01T00:00:00+00:00"),
                    false)
            },
            "{\"items\":[{\"etag\":\"TestEtag\",\"key\":\"TestKey\",\"label\":null,\"content_type\":null,\"value\":null,\"tags\":{},\"locked\":false,\"last_modified\":\"2023-10-01T00:00:00.0000000\\u002B00:00\"}]}"
        },
        new object?[]
        {
            new ConfigurationSetting[]
            {
                new(
                    "TestEtag",
                    "TestKey",
                    DateTimeOffset.Parse("2023-10-01T00:00:00+00:00"),
                    false,
                    label: "TestLabel")
            },
            "{\"items\":[{\"etag\":\"TestEtag\",\"key\":\"TestKey\",\"label\":\"TestLabel\",\"content_type\":null,\"value\":null,\"tags\":{},\"locked\":false,\"last_modified\":\"2023-10-01T00:00:00.0000000\\u002B00:00\"}]}"
        },
        new object?[]
        {
            new ConfigurationSetting[]
            {
                new(
                    "TestEtag",
                    "TestKey",
                    DateTimeOffset.Parse("2023-10-01T00:00:00+00:00"),
                    false,
                    contentType: "TestContentType")
            },
            "{\"items\":[{\"etag\":\"TestEtag\",\"key\":\"TestKey\",\"label\":null,\"content_type\":\"TestContentType\",\"value\":null,\"tags\":{},\"locked\":false,\"last_modified\":\"2023-10-01T00:00:00.0000000\\u002B00:00\"}]}"
        },
        new object?[]
        {
            new ConfigurationSetting[]
            {
                new(
                    "TestEtag",
                    "TestKey",
                    DateTimeOffset.Parse("2023-10-01T00:00:00+00:00"),
                    false,
                    value: "TestValue")
            },
            "{\"items\":[{\"etag\":\"TestEtag\",\"key\":\"TestKey\",\"label\":null,\"content_type\":null,\"value\":\"TestValue\",\"tags\":{},\"locked\":false,\"last_modified\":\"2023-10-01T00:00:00.0000000\\u002B00:00\"}]}"
        },
        new object?[]
        {
            new ConfigurationSetting[]
            {
                new(
                    "TestEtag",
                    "TestKey",
                    DateTimeOffset.Parse("2023-10-01T00:00:00+00:00"),
                    false,
                    tags: new Dictionary<string, string> { { "TestKey", "TestValue" } })
            },
            "{\"items\":[{\"etag\":\"TestEtag\",\"key\":\"TestKey\",\"label\":null,\"content_type\":null,\"value\":null,\"tags\":{\"TestKey\":\"TestValue\"},\"locked\":false,\"last_modified\":\"2023-10-01T00:00:00.0000000\\u002B00:00\"}]}"
        },
        new object?[]
        {
            new ConfigurationSetting[]
            {
                new(
                    "TestEtag",
                    "TestKey",
                    DateTimeOffset.Parse("2023-10-01T00:00:00+00:00"),
                    false,
                    "TestLabel",
                    "TestContentType",
                    "TestValue",
                    new Dictionary<string, string> { { "TestKey", "TestValue" } })
            },
            "{\"items\":[{\"etag\":\"TestEtag\",\"key\":\"TestKey\",\"label\":\"TestLabel\",\"content_type\":\"TestContentType\",\"value\":\"TestValue\",\"tags\":{\"TestKey\":\"TestValue\"},\"locked\":false,\"last_modified\":\"2023-10-01T00:00:00.0000000\\u002B00:00\"}]}"
        },
        new object?[]
        {
            new ConfigurationSetting[]
            {
                new(
                    "TestEtag",
                    "TestKey1",
                    DateTimeOffset.Parse("2023-10-01T00:00:00+00:00"),
                    false),
                new(
                    "TestEtag",
                    "TestKey2",
                    DateTimeOffset.Parse("2023-10-01T00:00:00+00:00"),
                    false)
            },
            "{\"items\":[{\"etag\":\"TestEtag\",\"key\":\"TestKey1\",\"label\":null,\"content_type\":null,\"value\":null,\"tags\":{},\"locked\":false,\"last_modified\":\"2023-10-01T00:00:00.0000000\\u002B00:00\"},{\"etag\":\"TestEtag\",\"key\":\"TestKey2\",\"label\":null,\"content_type\":null,\"value\":null,\"tags\":{},\"locked\":false,\"last_modified\":\"2023-10-01T00:00:00.0000000\\u002B00:00\"}]}"
        },
        new object?[]
        {
            new ConfigurationSetting[]
            {
                new(
                    "TestEtag",
                    "TestKey1",
                    DateTimeOffset.Parse("2023-10-01T00:00:00+00:00"),
                    false,
                    label: "TestLabel"),
                new(
                    "TestEtag",
                    "TestKey2",
                    DateTimeOffset.Parse("2023-10-01T00:00:00+00:00"),
                    false,
                    label: "TestLabel")
            },
            "{\"items\":[{\"etag\":\"TestEtag\",\"key\":\"TestKey1\",\"label\":\"TestLabel\",\"content_type\":null,\"value\":null,\"tags\":{},\"locked\":false,\"last_modified\":\"2023-10-01T00:00:00.0000000\\u002B00:00\"},{\"etag\":\"TestEtag\",\"key\":\"TestKey2\",\"label\":\"TestLabel\",\"content_type\":null,\"value\":null,\"tags\":{},\"locked\":false,\"last_modified\":\"2023-10-01T00:00:00.0000000\\u002B00:00\"}]}"
        },
        new object?[]
        {
            new ConfigurationSetting[]
            {
                new(
                    "TestEtag",
                    "TestKey1",
                    DateTimeOffset.Parse("2023-10-01T00:00:00+00:00"),
                    false,
                    contentType: "TestContentType"),
                new(
                    "TestEtag",
                    "TestKey2",
                    DateTimeOffset.Parse("2023-10-01T00:00:00+00:00"),
                    false,
                    contentType: "TestContentType")
            },
            "{\"items\":[{\"etag\":\"TestEtag\",\"key\":\"TestKey1\",\"label\":null,\"content_type\":\"TestContentType\",\"value\":null,\"tags\":{},\"locked\":false,\"last_modified\":\"2023-10-01T00:00:00.0000000\\u002B00:00\"},{\"etag\":\"TestEtag\",\"key\":\"TestKey2\",\"label\":null,\"content_type\":\"TestContentType\",\"value\":null,\"tags\":{},\"locked\":false,\"last_modified\":\"2023-10-01T00:00:00.0000000\\u002B00:00\"}]}"
        },
        new object?[]
        {
            new ConfigurationSetting[]
            {
                new(
                    "TestEtag",
                    "TestKey1",
                    DateTimeOffset.Parse("2023-10-01T00:00:00+00:00"),
                    false,
                    value: "TestValue"),
                new(
                    "TestEtag",
                    "TestKey2",
                    DateTimeOffset.Parse("2023-10-01T00:00:00+00:00"),
                    false,
                    value: "TestValue")
            },
            "{\"items\":[{\"etag\":\"TestEtag\",\"key\":\"TestKey1\",\"label\":null,\"content_type\":null,\"value\":\"TestValue\",\"tags\":{},\"locked\":false,\"last_modified\":\"2023-10-01T00:00:00.0000000\\u002B00:00\"},{\"etag\":\"TestEtag\",\"key\":\"TestKey2\",\"label\":null,\"content_type\":null,\"value\":\"TestValue\",\"tags\":{},\"locked\":false,\"last_modified\":\"2023-10-01T00:00:00.0000000\\u002B00:00\"}]}"
        },
        new object?[]
        {
            new ConfigurationSetting[]
            {
                new(
                    "TestEtag",
                    "TestKey1",
                    DateTimeOffset.Parse("2023-10-01T00:00:00+00:00"),
                    false,
                    tags: new Dictionary<string, string> { { "TestKey", "TestValue" } }),
                new(
                    "TestEtag",
                    "TestKey2",
                    DateTimeOffset.Parse("2023-10-01T00:00:00+00:00"),
                    false,
                    tags: new Dictionary<string, string> { { "TestKey", "TestValue" } })
            },
            "{\"items\":[{\"etag\":\"TestEtag\",\"key\":\"TestKey1\",\"label\":null,\"content_type\":null,\"value\":null,\"tags\":{\"TestKey\":\"TestValue\"},\"locked\":false,\"last_modified\":\"2023-10-01T00:00:00.0000000\\u002B00:00\"},{\"etag\":\"TestEtag\",\"key\":\"TestKey2\",\"label\":null,\"content_type\":null,\"value\":null,\"tags\":{\"TestKey\":\"TestValue\"},\"locked\":false,\"last_modified\":\"2023-10-01T00:00:00.0000000\\u002B00:00\"}]}"
        },
        new object?[]
        {
            new ConfigurationSetting[]
            {
                new(
                    "TestEtag",
                    "TestKey1",
                    DateTimeOffset.Parse("2023-10-01T00:00:00+00:00"),
                    false,
                    "TestLabel",
                    "TestContentType",
                    "TestValue",
                    new Dictionary<string, string> { { "TestKey", "TestValue" } }),
                new(
                    "TestEtag",
                    "TestKey2",
                    DateTimeOffset.Parse("2023-10-01T00:00:00+00:00"),
                    false,
                    "TestLabel",
                    "TestContentType",
                    "TestValue",
                    new Dictionary<string, string> { { "TestKey", "TestValue" } })
            },
            "{\"items\":[{\"etag\":\"TestEtag\",\"key\":\"TestKey1\",\"label\":\"TestLabel\",\"content_type\":\"TestContentType\",\"value\":\"TestValue\",\"tags\":{\"TestKey\":\"TestValue\"},\"locked\":false,\"last_modified\":\"2023-10-01T00:00:00.0000000\\u002B00:00\"},{\"etag\":\"TestEtag\",\"key\":\"TestKey2\",\"label\":\"TestLabel\",\"content_type\":\"TestContentType\",\"value\":\"TestValue\",\"tags\":{\"TestKey\":\"TestValue\"},\"locked\":false,\"last_modified\":\"2023-10-01T00:00:00.0000000\\u002B00:00\"}]}"
        }
    ];

    [Test]
    public async Task ExecuteAsync_ResponseStatusCode_ConfigurationSettings()
    {
        // Arrange
        var settings = new List<ConfigurationSetting> { new("TestEtag", "TestKey", DateTimeOffset.UtcNow, false) };

        // Act
        await new ConfigurationSettingsResult(settings).ExecuteAsync(HttpContext);

        // Assert
        Assert.That(HttpContext.Response.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
    }
}
