using AzureAppConfigurationEmulator.Common.Models;
using Microsoft.Data.Sqlite;
using NUnit.Framework;

namespace AzureAppConfigurationEmulator.Data.Sqlite.Tests;

public class SqliteDbParameterFactoryTests
{
    [TestCaseSource(nameof(Create_DbParameter_ConfigurationSettingContentType_TestCases))]
    public void Create_DbParameter_ConfigurationSettingContentType(string? contentType, object expected)
    {
        // Arrange
        var factory = new SqliteDbParameterFactory();
        var setting = new ConfigurationSetting("TestEtag", "TestKey", DateTimeOffset.UtcNow, false, contentType: contentType);

        // Act
        var parameter = (SqliteParameter)factory.Create("TestName", setting.ContentType);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(parameter.SqliteType, Is.EqualTo(SqliteType.Text));
            Assert.That(parameter.Value, Is.EqualTo(expected));
        });
    }

    // ReSharper disable once InconsistentNaming
    private static object[] Create_DbParameter_ConfigurationSettingContentType_TestCases =
    [
        new object?[] { "TestContentType", "TestContentType" },
        new object?[] { null, DBNull.Value }
    ];

    [Test]
    public void Create_DbParameter_ConfigurationSettingEtag()
    {
        // Arrange
        var factory = new SqliteDbParameterFactory();
        const string etag = "TestEtag";
        var setting = new ConfigurationSetting(etag, "TestKey", DateTimeOffset.UtcNow, false);

        // Act
        var parameter = (SqliteParameter)factory.Create("TestName", setting.Etag);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(parameter.SqliteType, Is.EqualTo(SqliteType.Text));
            Assert.That(parameter.Value, Is.EqualTo(etag));
        });
    }

    [Test]
    public void Create_DbParameter_ConfigurationSettingKey()
    {
        // Arrange
        var factory = new SqliteDbParameterFactory();
        const string key = "TestKey";
        var setting = new ConfigurationSetting("TestEtag", key, DateTimeOffset.UtcNow, false);

        // Act
        var parameter = (SqliteParameter)factory.Create("TestName", setting.Key);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(parameter.SqliteType, Is.EqualTo(SqliteType.Text));
            Assert.That(parameter.Value, Is.EqualTo(key));
        });
    }

    [TestCaseSource(nameof(Create_DbParameter_ConfigurationSettingLabel_TestCases))]
    public void Create_DbParameter_ConfigurationSettingLabel(string? label, object expected)
    {
        // Arrange
        var factory = new SqliteDbParameterFactory();
        var setting = new ConfigurationSetting("TestEtag", "TestKey", DateTimeOffset.UtcNow, false, label: label);

        // Act
        var parameter = (SqliteParameter)factory.Create("TestName", setting.Label);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(parameter.SqliteType, Is.EqualTo(SqliteType.Text));
            Assert.That(parameter.Value, Is.EqualTo(expected));
        });
    }

    // ReSharper disable once InconsistentNaming
    private static object[] Create_DbParameter_ConfigurationSettingLabel_TestCases =
    [
        new object?[] { "TestLabel", "TestLabel" },
        new object?[] { null, DBNull.Value }
    ];

    [Test]
    public void Create_DbParameter_ConfigurationSettingLastModified()
    {
        // Arrange
        var factory = new SqliteDbParameterFactory();
        var lastModified = DateTimeOffset.UtcNow;
        var setting = new ConfigurationSetting("TestEtag", "TestKey", lastModified, false);

        // Act
        var parameter = (SqliteParameter)factory.Create("TestName", setting.LastModified);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(parameter.SqliteType, Is.EqualTo(SqliteType.Text));
            Assert.That(parameter.Value, Is.EqualTo(lastModified.UtcDateTime.ToString("yyyy-MM-dd HH:mm:ss")));
        });
    }

    [TestCase(false, 0)]
    [TestCase(true, 1)]
    public void Create_DbParameter_ConfigurationSettingLocked(bool locked, int expected)
    {
        // Arrange
        var factory = new SqliteDbParameterFactory();
        var setting = new ConfigurationSetting("TestEtag", "TestKey", DateTimeOffset.UtcNow, locked);

        // Act
        var parameter = (SqliteParameter)factory.Create("TestName", setting.Locked);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(parameter.SqliteType, Is.EqualTo(SqliteType.Integer));
            Assert.That(parameter.Value, Is.EqualTo(expected));
        });
    }

    [TestCaseSource(nameof(Create_DbParameter_ConfigurationSettingTags_TestCases))]
    public void Create_DbParameter_ConfigurationSettingTags(IDictionary<string, string>? tags, object expected)
    {
        // Arrange
        var factory = new SqliteDbParameterFactory();
        var setting = new ConfigurationSetting("TestEtag", "TestKey", DateTimeOffset.UtcNow, false, tags: tags);

        // Act
        var parameter = (SqliteParameter)factory.Create("TestName", setting.Tags);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(parameter.SqliteType, Is.EqualTo(SqliteType.Text));
            Assert.That(parameter.Value, Is.EqualTo(expected));
        });
    }

    // ReSharper disable once InconsistentNaming
    private static object[] Create_DbParameter_ConfigurationSettingTags_TestCases =
    [
        new object?[] { new Dictionary<string, string> { { "TestKey", "TestValue" } }, "{\"TestKey\":\"TestValue\"}" },
        new object?[] { null, DBNull.Value }
    ];

    [TestCaseSource(nameof(Create_DbParameter_ConfigurationSettingValue_TestCases))]
    public void Create_DbParameter_ConfigurationSettingValue(string? value, object expected)
    {
        // Arrange
        var factory = new SqliteDbParameterFactory();
        var setting = new ConfigurationSetting("TestEtag", "TestKey", DateTimeOffset.UtcNow, false, value: value);

        // Act
        var parameter = (SqliteParameter)factory.Create("TestName", setting.Value);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(parameter.SqliteType, Is.EqualTo(SqliteType.Text));
            Assert.That(parameter.Value, Is.EqualTo(expected));
        });
    }

    // ReSharper disable once InconsistentNaming
    private static object[] Create_DbParameter_ConfigurationSettingValue_TestCases =
    [
        new object?[] { "TestValue", "TestValue" },
        new object?[] { null, DBNull.Value }
    ];

    [Test]
    public void Create_DbParameterName_NameAndValue()
    {
        // Arrange
        var factory = new SqliteDbParameterFactory();

        // Act
        var parameter = (SqliteParameter)factory.Create("TestName", "TestValue");

        // Assert
        Assert.That(parameter.ParameterName, Is.EqualTo("TestName"));
    }

    [TestCaseSource(nameof(Create_DbParameterType_NameAndValue_TestCases))]
    public void Create_DbParameterType_NameAndValue(object? value, SqliteType expected)
    {
        // Arrange
        var factory = new SqliteDbParameterFactory();

        // Act
        var parameter = (SqliteParameter)factory.Create("TestName", value);

        // Assert
        Assert.That(parameter.SqliteType, Is.EqualTo(expected));
    }

    // ReSharper disable once InconsistentNaming
    private static object[] Create_DbParameterType_NameAndValue_TestCases =
    [
        new object?[] { true, SqliteType.Integer },
        new object?[] { false, SqliteType.Integer },
        new object?[] { DateTime.Parse("2023-10-01T00:00:00+00:00"), SqliteType.Text },
        new object?[] { DateTime.Parse("2023-10-01T12:00:00+12:00"), SqliteType.Text },
        new object?[] { DateTimeOffset.Parse("2023-10-01T00:00:00+00:00"), SqliteType.Text },
        new object?[] { DateTimeOffset.Parse("2023-10-01T12:00:00+12:00"), SqliteType.Text },
        new object?[] { new Dictionary<string, object?> { { "TestKey", "TestValue" } }, SqliteType.Text },
        new object?[] { new Dictionary<string, object?> { { "TestKey", null } }, SqliteType.Text },
        new object?[] { 0, SqliteType.Integer },
        new object?[] { null, SqliteType.Text },
        new object?[] { "Hello World", SqliteType.Text }
    ];

    [TestCaseSource(nameof(Create_DbParameterValue_NameAndValue_TestCases))]
    public void Create_DbParameterValue_NameAndValue(object? value, object expected)
    {
        // Arrange
        var factory = new SqliteDbParameterFactory();

        // Act
        var parameter = (SqliteParameter)factory.Create("TestName", value);

        // Assert
        Assert.That(parameter.Value, Is.EqualTo(expected));
    }

    // ReSharper disable once InconsistentNaming
    private static object[] Create_DbParameterValue_NameAndValue_TestCases =
    [
        new object?[] { true, 1 },
        new object?[] { false, 0 },
        new object?[] { DateTime.Parse("2023-10-01T00:00:00+00:00"), "2023-10-01 00:00:00" },
        new object?[] { DateTime.Parse("2023-10-01T12:00:00+12:00"), "2023-10-01 00:00:00" },
        new object?[] { DateTimeOffset.Parse("2023-10-01T00:00:00+00:00"), "2023-10-01 00:00:00" },
        new object?[] { DateTimeOffset.Parse("2023-10-01T12:00:00+12:00"), "2023-10-01 00:00:00" },
        new object?[] { new Dictionary<string, object?> { { "TestKey", "TestValue" } }, "{\"TestKey\":\"TestValue\"}" },
        new object?[] { new Dictionary<string, object?> { { "TestKey", null } }, "{\"TestKey\":null}" },
        new object?[] { 0, 0 },
        new object?[] { null, DBNull.Value },
        new object?[] { "Hello World", "Hello World" }
    ];

    [Test]
    public void Create_ArgumentOutOfRangeException_NonSupportedType()
    {
        // Arrange
        var factory = new SqliteDbParameterFactory();

        // Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => factory.Create("TestName", 0.0));
    }
}
