using Microsoft.Data.Sqlite;
using NUnit.Framework;

namespace AzureAppConfigurationEmulator.Data.Sqlite.Tests;

public class SqliteDbParameterFactoryTests
{
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
        new object?[] { "Hello World", SqliteType.Text },
        new object?[] { 0.0, SqliteType.Text }
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
        new object?[] { "Hello World", "Hello World" },
        new object?[] { 0.0, "0" }
    ];
}
