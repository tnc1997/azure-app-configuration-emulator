using AzureAppConfigurationEmulator.ConfigurationSettings;
using NUnit.Framework;

namespace AzureAppConfigurationEmulator.Tests.ConfigurationSettings;

public class FeatureFlagConfigurationSettingTests
{
    [Test]
    public void Constructor_ValueWithClientFiltersAsEmptyList_InitializesClientFiltersAsEmptyList()
    {
        // Arrange
        const string value = "{\"id\":\"TestId\",\"enabled\":false,\"conditions\":{\"client_filters\":[]}}";

        // Act
        var setting = new FeatureFlagConfigurationSetting("TestEtag", "TestKey", value, DateTimeOffset.UtcNow, false);

        // Assert
        Assert.That(setting.ClientFilters, Is.Empty);
    }

    [Test]
    public void Constructor_ValueWithClientFiltersAsNull_InitializesClientFiltersAsEmptyList()
    {
        // Arrange
        const string value = "{\"id\":\"TestId\",\"enabled\":false,\"conditions\":{\"client_filters\":null}}";

        // Act
        var setting = new FeatureFlagConfigurationSetting("TestEtag", "TestKey", value, DateTimeOffset.UtcNow, false);

        // Assert
        Assert.That(setting.ClientFilters, Is.Empty);
    }

    [Test]
    public void Constructor_ValueWithClientFiltersWithMultipleClientFilters_InitializesClientFiltersWithMultipleClientFilters()
    {
        // Arrange
        const string value = "{\"id\":\"TestId\",\"enabled\":false,\"conditions\":{\"client_filters\":[{\"name\":\"TestName1\",\"parameters\":{}},{\"name\":\"TestName2\",\"parameters\":{}}]}}";

        // Act
        var setting = new FeatureFlagConfigurationSetting("TestEtag", "TestKey", value, DateTimeOffset.UtcNow, false);

        // Assert
        Assert.That(setting.ClientFilters, Has.Count.EqualTo(2));
    }

    [Test]
    public void Constructor_ValueWithClientFiltersWithSingleClientFilter_InitializesClientFiltersWithSingleClientFilter()
    {
        // Arrange
        const string value = "{\"id\":\"TestId\",\"enabled\":false,\"conditions\":{\"client_filters\":[{\"name\":\"TestName\",\"parameters\":{}}]}}";

        // Act
        var setting = new FeatureFlagConfigurationSetting("TestEtag", "TestKey", value, DateTimeOffset.UtcNow, false);

        // Assert
        Assert.That(setting.ClientFilters, Has.Count.EqualTo(1));
    }

    [Test]
    public void Constructor_ValueWithClientFiltersWithSingleClientFilterWithMultipleParameters_InitializesClientFiltersWithSingleClientFilterWithMultipleParameters()
    {
        // Arrange
        var parameter1 = KeyValuePair.Create("TestKey1", "TestValue1");
        var parameter2 = KeyValuePair.Create("TestKey2", "TestValue2");
        var value = $"{{\"id\":\"TestId\",\"enabled\":false,\"conditions\":{{\"client_filters\":[{{\"name\":\"TestName\",\"parameters\":{{\"{parameter1.Key}\":\"{parameter1.Value}\",\"{parameter2.Key}\":\"{parameter2.Value}\"}}}}]}}}}";

        // Act
        var setting = new FeatureFlagConfigurationSetting("TestEtag", "TestKey", value, DateTimeOffset.UtcNow, false);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(setting.ClientFilters.ElementAt(0).Parameters.ElementAt(0).Key, Is.EqualTo(parameter1.Key));
            Assert.That(setting.ClientFilters.ElementAt(0).Parameters.ElementAt(0).Value, Is.EqualTo(parameter1.Value));
            Assert.That(setting.ClientFilters.ElementAt(0).Parameters.ElementAt(1).Key, Is.EqualTo(parameter2.Key));
            Assert.That(setting.ClientFilters.ElementAt(0).Parameters.ElementAt(1).Value, Is.EqualTo(parameter2.Value));
        });
    }

    [Test]
    public void Constructor_ValueWithClientFiltersWithSingleClientFilterWithName_InitializesClientFiltersWithSingleClientFilterWithName()
    {
        // Arrange
        const string name = "TestName";
        const string value = $"{{\"id\":\"TestId\",\"enabled\":false,\"conditions\":{{\"client_filters\":[{{\"name\":\"{name}\",\"parameters\":{{}}}}]}}}}";

        // Act
        var setting = new FeatureFlagConfigurationSetting("TestEtag", "TestKey", value, DateTimeOffset.UtcNow, false);

        // Assert
        Assert.That(setting.ClientFilters.ElementAt(0).Name, Is.EqualTo(name));
    }

    [Test]
    public void Constructor_ValueWithClientFiltersWithSingleClientFilterWithSingleParameter_InitializesClientFiltersWithSingleClientFilterWithSingleParameter()
    {
        // Arrange
        var parameter = KeyValuePair.Create("TestKey", "TestValue");
        var value = $"{{\"id\":\"TestId\",\"enabled\":false,\"conditions\":{{\"client_filters\":[{{\"name\":\"TestName\",\"parameters\":{{\"{parameter.Key}\":\"{parameter.Value}\"}}}}]}}}}";

        // Act
        var setting = new FeatureFlagConfigurationSetting("TestEtag", "TestKey", value, DateTimeOffset.UtcNow, false);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(setting.ClientFilters.ElementAt(0).Parameters.ElementAt(0).Key, Is.EqualTo(parameter.Key));
            Assert.That(setting.ClientFilters.ElementAt(0).Parameters.ElementAt(0).Value, Is.EqualTo(parameter.Value));
        });
    }

    [TestCase("\"TestDescription\"", "TestDescription")]
    [TestCase("null", null)]
    public void Constructor_ValueWithDescription_InitializesDescription(string description, string? expected)
    {
        // Arrange
        var value = $"{{\"id\":\"TestId\",\"enabled\":false,\"conditions\":{{\"client_filters\":[]}},\"description\":{description}}}";

        // Act
        var setting = new FeatureFlagConfigurationSetting("TestEtag", "TestKey", value, DateTimeOffset.UtcNow, false);

        // Assert
        Assert.That(setting.Description, Is.EqualTo(expected));
    }

    [TestCase("\"TestDisplayName\"", "TestDisplayName")]
    [TestCase("null", null)]
    public void Constructor_ValueWithDisplayName_InitializesDisplayName(string displayName, string? expected)
    {
        // Arrange
        var value = $"{{\"id\":\"TestId\",\"enabled\":false,\"conditions\":{{\"client_filters\":[]}},\"display_name\":{displayName}}}";

        // Act
        var setting = new FeatureFlagConfigurationSetting("TestEtag", "TestKey", value, DateTimeOffset.UtcNow, false);

        // Assert
        Assert.That(setting.DisplayName, Is.EqualTo(expected));
    }

    [TestCase("false", false)]
    [TestCase("true", true)]
    public void Constructor_ValueWithEnabled_InitializesEnabled(string enabled, bool expected)
    {
        // Arrange
        var value = $"{{\"id\":\"TestId\",\"enabled\":{enabled},\"conditions\":{{\"client_filters\":[]}}}}";

        // Act
        var setting = new FeatureFlagConfigurationSetting("TestEtag", "TestKey", value, DateTimeOffset.UtcNow, false);

        // Assert
        Assert.That(setting.Enabled, Is.EqualTo(expected));
    }

    [TestCase("\"TestId\"", "TestId")]
    public void Constructor_ValueWithId_InitializesId(string id, string expected)
    {
        // Arrange
        var value = $"{{\"id\":{id},\"enabled\":false,\"conditions\":{{\"client_filters\":[]}}}}";

        // Act
        var setting = new FeatureFlagConfigurationSetting("TestEtag", "TestKey", value, DateTimeOffset.UtcNow, false);

        // Assert
        Assert.That(setting.Id, Is.EqualTo(expected));
    }

    [TestCaseSource(nameof(ValueGetter_Value_SerializesValue_TestCases))]
    public void ValueGetter_Value_SerializesValue(string id, bool enabled, ICollection<FeatureFlagFilter> clientFilters, string? description, string? displayName, string expected)
    {
        // Arrange
        var setting = new FeatureFlagConfigurationSetting(id, enabled, clientFilters, "TestEtag", "TestKey", DateTimeOffset.UtcNow, false, description, displayName);

        // Act
        var value = setting.Value;

        // Assert
        Assert.That(value, Is.EqualTo(expected));
    }

    // ReSharper disable once InconsistentNaming
    private static object[] ValueGetter_Value_SerializesValue_TestCases =
    [
        new object?[]
        {
            "TestId",
            false,
            new List<FeatureFlagFilter>(),
            "TestDescription",
            "TestDisplayName",
            "{\"id\":\"TestId\",\"enabled\":false,\"conditions\":{\"client_filters\":[]},\"description\":\"TestDescription\",\"display_name\":\"TestDisplayName\"}"
        },
        new object?[]
        {
            "TestId",
            false,
            new List<FeatureFlagFilter>
            {
                new("TestName", new Dictionary<string, object>())
            },
            "TestDescription",
            "TestDisplayName",
            "{\"id\":\"TestId\",\"enabled\":false,\"conditions\":{\"client_filters\":[{\"name\":\"TestName\",\"parameters\":{}}]},\"description\":\"TestDescription\",\"display_name\":\"TestDisplayName\"}"
        },
        new object?[]
        {
            "TestId",
            false,
            new List<FeatureFlagFilter>
            {
                new("TestName", new Dictionary<string, object>
                {
                    { "TestKey", "TestValue" }
                })
            },
            "TestDescription",
            "TestDisplayName",
            "{\"id\":\"TestId\",\"enabled\":false,\"conditions\":{\"client_filters\":[{\"name\":\"TestName\",\"parameters\":{\"TestKey\":\"TestValue\"}}]},\"description\":\"TestDescription\",\"display_name\":\"TestDisplayName\"}"
        },
        new object?[]
        {
            "TestId",
            false,
            new List<FeatureFlagFilter>
            {
                new("TestName", new Dictionary<string, object>
                {
                    { "TestKey1", "TestValue1" },
                    { "TestKey2", "TestValue2" }
                })
            },
            "TestDescription",
            "TestDisplayName",
            "{\"id\":\"TestId\",\"enabled\":false,\"conditions\":{\"client_filters\":[{\"name\":\"TestName\",\"parameters\":{\"TestKey1\":\"TestValue1\",\"TestKey2\":\"TestValue2\"}}]},\"description\":\"TestDescription\",\"display_name\":\"TestDisplayName\"}"
        },
        new object?[]
        {
            "TestId",
            false,
            new List<FeatureFlagFilter>
            {
                new("TestName1", new Dictionary<string, object>()),
                new("TestName2", new Dictionary<string, object>())
            },
            "TestDescription",
            "TestDisplayName",
            "{\"id\":\"TestId\",\"enabled\":false,\"conditions\":{\"client_filters\":[{\"name\":\"TestName1\",\"parameters\":{}},{\"name\":\"TestName2\",\"parameters\":{}}]},\"description\":\"TestDescription\",\"display_name\":\"TestDisplayName\"}"
        },
        new object?[]
        {
            "TestId",
            false,
            new List<FeatureFlagFilter>
            {
                new("TestName1", new Dictionary<string, object>
                {
                    { "TestKey1", "TestValue1" }
                }),
                new("TestName2", new Dictionary<string, object>
                {
                    { "TestKey2", "TestValue2" }
                })
            },
            "TestDescription",
            "TestDisplayName",
            "{\"id\":\"TestId\",\"enabled\":false,\"conditions\":{\"client_filters\":[{\"name\":\"TestName1\",\"parameters\":{\"TestKey1\":\"TestValue1\"}},{\"name\":\"TestName2\",\"parameters\":{\"TestKey2\":\"TestValue2\"}}]},\"description\":\"TestDescription\",\"display_name\":\"TestDisplayName\"}"
        },
        new object?[]
        {
            "TestId",
            false,
            new List<FeatureFlagFilter>
            {
                new("TestName1", new Dictionary<string, object>
                {
                    { "TestKey1", "TestValue1" },
                    { "TestKey2", "TestValue2" }
                }),
                new("TestName2", new Dictionary<string, object>
                {
                    { "TestKey3", "TestValue3" },
                    { "TestKey4", "TestValue4" }
                })
            },
            "TestDescription",
            "TestDisplayName",
            "{\"id\":\"TestId\",\"enabled\":false,\"conditions\":{\"client_filters\":[{\"name\":\"TestName1\",\"parameters\":{\"TestKey1\":\"TestValue1\",\"TestKey2\":\"TestValue2\"}},{\"name\":\"TestName2\",\"parameters\":{\"TestKey3\":\"TestValue3\",\"TestKey4\":\"TestValue4\"}}]},\"description\":\"TestDescription\",\"display_name\":\"TestDisplayName\"}"
        },
        new object?[]
        {
            "TestId",
            false,
            new List<FeatureFlagFilter>(),
            null,
            null,
            "{\"id\":\"TestId\",\"enabled\":false,\"conditions\":{\"client_filters\":[]}}"
        }
    ];
}
