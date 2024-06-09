using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using AzureAppConfigurationEmulator.Common;
using AzureAppConfigurationEmulator.ConfigurationSettings;
using NUnit.Framework;

namespace AzureAppConfigurationEmulator.Tests.Common;

public class SelectJsonTypeInfoModifierTests
{
    private JsonTypeInfo Type { get; set; }

    [SetUp]
    public void SetUp()
    {
        Type = JsonTypeInfo.CreateJsonTypeInfo<ConfigurationSetting>(JsonSerializerOptions.Default);
        
        foreach (var property in typeof(ConfigurationSetting).GetProperties())
        {
            Type.Properties.Add(Type.CreateJsonPropertyInfo(property.PropertyType, property.Name));
        }
    }

    [TestCaseSource(nameof(Modify_JsonTypeInfo_Names_TestCases))]
    public void Modify_JsonTypeInfo_Names(string[] names, string[] expected)
    {
        // Arrange
        var modifier = new SelectJsonTypeInfoModifier(names);

        // Act
        modifier.Modify(Type);

        // Assert
        Assert.That(Type.Properties.Select(property => property.Name).ToArray(), Is.EqualTo(expected));
    }

    // ReSharper disable once InconsistentNaming
    private static object[] Modify_JsonTypeInfo_Names_TestCases =
    [
        new object?[]
        {
            new[] { nameof(ConfigurationSetting.Key) },
            new[] { nameof(ConfigurationSetting.Key) }
        },
        new object?[]
        {
            new[] { nameof(ConfigurationSetting.Key), nameof(ConfigurationSetting.Value) },
            new[] { nameof(ConfigurationSetting.Key), nameof(ConfigurationSetting.Value) }
        },
        new object?[]
        {
            Array.Empty<string>(),
            typeof(ConfigurationSetting).GetProperties().Select(property => property.Name).ToArray()
        },
        new object?[]
        {
            null,
            typeof(ConfigurationSetting).GetProperties().Select(property => property.Name).ToArray()
        }
    ];
}
