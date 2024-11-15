using AzureAppConfigurationEmulator.ConfigurationSettings;
using NUnit.Framework;

namespace AzureAppConfigurationEmulator.Tests.ConfigurationSettings;

public class ConfigurationSettingFactoryTests
{
    private ConfigurationSettingFactory Factory { get; set; }

    [SetUp]
    public void SetUp()
    {
        Factory = new ConfigurationSettingFactory();
    }

    [TestCase("application/json", typeof(ConfigurationSetting))]
    [TestCase("application/json;charset=utf-8", typeof(ConfigurationSetting))]
    [TestCase("application/vnd.microsoft.appconfig.ff+json", typeof(FeatureFlagConfigurationSetting))]
    [TestCase("application/vnd.microsoft.appconfig.ff+json;charset=utf-8", typeof(FeatureFlagConfigurationSetting))]
    [TestCase("Invalid.Content.Type", typeof(ConfigurationSetting))]
    [TestCase(null, typeof(ConfigurationSetting))]
    public void Create_ConfigurationSetting_ContentType(string? contentType, Type expected)
    {
        // Arrange
        const string key = "TestKey";
        const string label = "TestLabel";
        const string value = "{\"id\":\"TestId\",\"enabled\":true}";

        // Act
        var setting = Factory.Create(key, label, contentType, value);

        // Assert
        Assert.That(setting, Is.TypeOf(expected));
    }
}
