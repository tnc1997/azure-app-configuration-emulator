using System.Text.Json;
using AzureAppConfigurationEmulator.Common;
using NUnit.Framework;

namespace AzureAppConfigurationEmulator.Tests.Common;

public class KeyValuePairJsonEncoderTests
{
    private KeyValuePairJsonEncoder Encoder { get; set; }

    [SetUp]
    public void SetUp()
    {
        Encoder = new KeyValuePairJsonEncoder();
    }

    [TestCaseSource(nameof(Encode_Document_KeyValuePairsAndPrefixAndSeparator_TestCases))]
    public void Encode_Document_KeyValuePairsAndPrefixAndSeparator(IEnumerable<KeyValuePair<string, string?>> pairs, string? prefix, string? separator, string expected)
    {
        // Act
        using var document = Encoder.Encode(pairs, prefix, separator);

        // Assert
        Assert.That(JsonSerializer.Serialize(document), Is.EqualTo(expected));
    }

    // ReSharper disable once InconsistentNaming
    private static object[] Encode_Document_KeyValuePairsAndPrefixAndSeparator_TestCases =
    [
        new object?[]
        {
            new Dictionary<string, string?> { { "TestKey", "TestValue" } },
            null,
            null,
            "{\"TestKey\":\"TestValue\"}"
        },
        new object?[]
        {
            new Dictionary<string, string?> { { "TestPrefixTestKey", "TestValue" } },
            "TestPrefix",
            null,
            "{\"TestKey\":\"TestValue\"}"
        },
        new object?[]
        {
            new Dictionary<string, string?> { { "TestKey", "TestValue" } },
            null,
            ".",
            "{\"TestKey\":\"TestValue\"}"
        },
        new object?[]
        {
            new Dictionary<string, string?> { { "TestPrefix.TestKey", "TestValue" } },
            "TestPrefix",
            ".",
            "{\"TestKey\":\"TestValue\"}"
        },
        new object?[]
        {
            new Dictionary<string, string?> { { "TestOuterKey.TestInnerKey", "TestValue" } },
            null,
            ".",
            "{\"TestOuterKey\":{\"TestInnerKey\":\"TestValue\"}}"
        },
        new object?[]
        {
            new Dictionary<string, string?> { { "TestPrefix.TestOuterKey.TestInnerKey", "TestValue" } },
            "TestPrefix",
            ".",
            "{\"TestOuterKey\":{\"TestInnerKey\":\"TestValue\"}}"
        },
        new object?[]
        {
            new Dictionary<string, string?> { { "TestKey.0", "TestValue" } },
            null,
            ".",
            "{\"TestKey\":[\"TestValue\"]}"
        },
        new object?[]
        {
            new Dictionary<string, string?> { { "TestPrefix.TestKey.0", "TestValue" } },
            "TestPrefix",
            ".",
            "{\"TestKey\":[\"TestValue\"]}"
        },
        new object?[]
        {
            new Dictionary<string, string?> { { "TestOuterKey.0.TestInnerKey", "TestValue" } },
            null,
            ".",
            "{\"TestOuterKey\":[{\"TestInnerKey\":\"TestValue\"}]}"
        },
        new object?[]
        {
            new Dictionary<string, string?> { { "TestPrefix.TestOuterKey.0.TestInnerKey", "TestValue" } },
            "TestPrefix",
            ".",
            "{\"TestOuterKey\":[{\"TestInnerKey\":\"TestValue\"}]}"
        }
    ];
}
