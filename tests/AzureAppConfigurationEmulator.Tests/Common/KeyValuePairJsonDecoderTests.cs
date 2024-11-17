using System.Text.Json;
using AzureAppConfigurationEmulator.Common;
using NUnit.Framework;

namespace AzureAppConfigurationEmulator.Tests.Common;

public class KeyValuePairJsonDecoderTests
{
    private KeyValuePairJsonDecoder Decoder { get; set; }

    [SetUp]
    public void SetUp()
    {
        Decoder = new KeyValuePairJsonDecoder();
    }

    [TestCaseSource(nameof(Decode_KeyValuePairs_DocumentAndPrefixAndSeparator_TestCases))]
    public void Decode_KeyValuePairs_DocumentAndPrefixAndSeparator(string json, string? prefix, string? separator, IEnumerable<KeyValuePair<string, string?>> expected)
    {
        // Arrange
        using var document = JsonDocument.Parse(json);

        // Act
        var settings = Decoder.Decode(document, prefix, separator);

        // Assert
        Assert.That(settings, Is.EqualTo(expected));
    }

    // ReSharper disable once InconsistentNaming
    private static object[] Decode_KeyValuePairs_DocumentAndPrefixAndSeparator_TestCases =
    [
        new object?[]
        {
            "{\"TestKey\":\"TestValue\"}",
            null,
            null,
            new Dictionary<string, string?> { { "TestKey", "TestValue" } }
        },
        new object?[]
        {
            "{\"TestKey\":\"TestValue\"}",
            "TestPrefix",
            null,
            new Dictionary<string, string?> { { "TestPrefixTestKey", "TestValue" } }
        },
        new object?[]
        {
            "{\"TestKey\":\"TestValue\"}",
            null,
            ".",
            new Dictionary<string, string?> { { "TestKey", "TestValue" } }
        },
        new object?[]
        {
            "{\"TestKey\":\"TestValue\"}",
            "TestPrefix",
            ".",
            new Dictionary<string, string?> { { "TestPrefix.TestKey", "TestValue" } }
        },
        new object?[]
        {
            "{\"TestOuterKey\":{\"TestInnerKey\":\"TestValue\"}}",
            null,
            null,
            new Dictionary<string, string?> { { "TestOuterKeyTestInnerKey", "TestValue" } }
        },
        new object?[]
        {
            "{\"TestOuterKey\":{\"TestInnerKey\":\"TestValue\"}}",
            "TestPrefix",
            null,
            new Dictionary<string, string?> { { "TestPrefixTestOuterKeyTestInnerKey", "TestValue" } }
        },
        new object?[]
        {
            "{\"TestOuterKey\":{\"TestInnerKey\":\"TestValue\"}}",
            null,
            ".",
            new Dictionary<string, string?> { { "TestOuterKey.TestInnerKey", "TestValue" } }
        },
        new object?[]
        {
            "{\"TestOuterKey\":{\"TestInnerKey\":\"TestValue\"}}",
            "TestPrefix",
            ".",
            new Dictionary<string, string?> { { "TestPrefix.TestOuterKey.TestInnerKey", "TestValue" } }
        },
        new object?[]
        {
            "{\"TestKey\":[\"TestValue\"]}",
            null,
            null,
            new Dictionary<string, string?> { { "TestKey0", "TestValue" } }
        },
        new object?[]
        {
            "{\"TestKey\":[\"TestValue\"]}",
            "TestPrefix",
            null,
            new Dictionary<string, string?> { { "TestPrefixTestKey0", "TestValue" } }
        },
        new object?[]
        {
            "{\"TestKey\":[\"TestValue\"]}",
            null,
            ".",
            new Dictionary<string, string?> { { "TestKey.0", "TestValue" } }
        },
        new object?[]
        {
            "{\"TestKey\":[\"TestValue\"]}",
            "TestPrefix",
            ".",
            new Dictionary<string, string?> { { "TestPrefix.TestKey.0", "TestValue" } }
        },
        new object?[]
        {
            "{\"TestOuterKey\":[{\"TestInnerKey\":\"TestValue\"}]}",
            null,
            null,
            new Dictionary<string, string?> { { "TestOuterKey0TestInnerKey", "TestValue" } }
        },
        new object?[]
        {
            "{\"TestOuterKey\":[{\"TestInnerKey\":\"TestValue\"}]}",
            "TestPrefix",
            null,
            new Dictionary<string, string?> { { "TestPrefixTestOuterKey0TestInnerKey", "TestValue" } }
        },
        new object?[]
        {
            "{\"TestOuterKey\":[{\"TestInnerKey\":\"TestValue\"}]}",
            null,
            ".",
            new Dictionary<string, string?> { { "TestOuterKey.0.TestInnerKey", "TestValue" } }
        },
        new object?[]
        {
            "{\"TestOuterKey\":[{\"TestInnerKey\":\"TestValue\"}]}",
            "TestPrefix",
            ".",
            new Dictionary<string, string?> { { "TestPrefix.TestOuterKey.0.TestInnerKey", "TestValue" } }
        }
    ];
}
