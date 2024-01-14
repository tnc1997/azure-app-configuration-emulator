using System.Data.Common;
using AzureAppConfigurationEmulator.Constants;
using AzureAppConfigurationEmulator.Entities;
using AzureAppConfigurationEmulator.Factories;
using AzureAppConfigurationEmulator.Repositories;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;

namespace AzureAppConfigurationEmulator.Tests.Repositories;

public class ConfigurationSettingRepositoryTests
{
    private IDbCommandFactory CommandFactory { get; set; }

    private IDbConnectionFactory ConnectionFactory { get; set; }

    private ILogger<ConfigurationSettingRepository> Logger { get; set; }

    private IDbParameterFactory ParameterFactory { get; set; }

    private ConfigurationSettingRepository Repository { get; set; }

    [SetUp]
    public void SetUp()
    {
        CommandFactory = Substitute.For<IDbCommandFactory>();
        CommandFactory
            .Create(Arg.Any<DbConnection>(), Arg.Any<string?>(), Arg.Any<IEnumerable<DbParameter>?>())
            .Returns(_ =>
            {
                var command = Substitute.For<DbCommand>();

                command.ExecuteReaderAsync().Returns(_ =>
                {
                    var reader = Substitute.For<DbDataReader>();
                    return reader;
                });

                return command;
            });

        ConnectionFactory = Substitute.For<IDbConnectionFactory>();
        ConnectionFactory
            .Create()
            .Returns(_ =>
            {
                var connection = Substitute.For<DbConnection>();
                return connection;
            });

        Logger = Substitute.For<ILogger<ConfigurationSettingRepository>>();

        ParameterFactory = Substitute.For<IDbParameterFactory>();

        Repository = new ConfigurationSettingRepository(CommandFactory, ConnectionFactory, Logger, ParameterFactory);
    }

    [Test]
    public async Task AddAsync_CommandText_ConfigurationSetting()
    {
        // Arrange
        var setting = new ConfigurationSetting("TestEtag", "TestKey", null, null, null, DateTimeOffset.UtcNow, false, null);

        // Act
        await Repository.AddAsync(setting);

        // Assert
        CommandFactory
            .Received()
            .Create(
                Arg.Any<DbConnection>(),
                Arg.Is("INSERT INTO configuration_settings (etag, key, label, content_type, value, last_modified, locked, tags) VALUES ($etag, $key, $label, $content_type, $value, $last_modified, $locked, $tags)"),
                Arg.Any<IEnumerable<DbParameter>?>());
    }

    [Test]
    public async Task AddAsync_ContentTypeParameter_ConfigurationSetting()
    {
        // Arrange
        const string contentType = "TestContentType";
        var setting = new ConfigurationSetting("TestEtag", "TestKey", null, contentType, null, DateTimeOffset.UtcNow, false, null);

        // Act
        await Repository.AddAsync(setting);

        // Assert
        ParameterFactory
            .Received()
            .Create(
                Arg.Is("$content_type"),
                Arg.Is(contentType));
    }

    [Test]
    public async Task AddAsync_EtagParameter_ConfigurationSetting()
    {
        // Arrange
        const string etag = "TestEtag";
        var setting = new ConfigurationSetting(etag, "TestKey", null, null, null, DateTimeOffset.UtcNow, false, null);

        // Act
        await Repository.AddAsync(setting);

        // Assert
        ParameterFactory
            .Received()
            .Create(
                Arg.Is("$etag"),
                Arg.Is(etag));
    }

    [Test]
    public async Task AddAsync_KeyParameter_ConfigurationSetting()
    {
        // Arrange
        const string key = "TestKey";
        var setting = new ConfigurationSetting("TestEtag", key, null, null, null, DateTimeOffset.UtcNow, false, null);

        // Act
        await Repository.AddAsync(setting);

        // Assert
        ParameterFactory
            .Received()
            .Create(
                Arg.Is("$key"),
                Arg.Is(key));
    }

    [Test]
    public async Task AddAsync_LabelParameter_ConfigurationSetting()
    {
        // Arrange
        const string label = "TestLabel";
        var setting = new ConfigurationSetting("TestEtag", "TestKey", label, null, null, DateTimeOffset.UtcNow, false, null);

        // Act
        await Repository.AddAsync(setting);

        // Assert
        ParameterFactory
            .Received()
            .Create(
                Arg.Is("$label"),
                Arg.Is(label));
    }

    [Test]
    public async Task AddAsync_LastModifiedParameter_ConfigurationSetting()
    {
        // Arrange
        var lastModified = DateTimeOffset.UtcNow;
        var setting = new ConfigurationSetting("TestEtag", "TestKey", null, null, null, lastModified, false, null);

        // Act
        await Repository.AddAsync(setting);

        // Assert
        ParameterFactory
            .Received()
            .Create(
                Arg.Is("$last_modified"),
                Arg.Is(lastModified));
    }

    [Test]
    public async Task AddAsync_LockedParameter_ConfigurationSetting()
    {
        // Arrange
        const bool locked = false;
        var setting = new ConfigurationSetting("TestEtag", "TestKey", null, null, null, DateTimeOffset.UtcNow, locked, null);

        // Act
        await Repository.AddAsync(setting);

        // Assert
        ParameterFactory
            .Received()
            .Create(
                Arg.Is("$locked"),
                Arg.Is(locked));
    }

    [Test]
    public async Task AddAsync_TagsParameter_ConfigurationSetting()
    {
        // Arrange
        var tags = new Dictionary<string, object?> { { "TestKey", "TestValue" } };
        var setting = new ConfigurationSetting("TestEtag", "TestKey", null, null, null, DateTimeOffset.UtcNow, false, tags);

        // Act
        await Repository.AddAsync(setting);

        // Assert
        ParameterFactory
            .Received()
            .Create(
                Arg.Is("$tags"),
                Arg.Is(tags));
    }

    [Test]
    public async Task AddAsync_ValueParameter_ConfigurationSetting()
    {
        // Arrange
        const string value = "TestValue";
        var setting = new ConfigurationSetting("TestEtag", "TestKey", null, null, value, DateTimeOffset.UtcNow, false, null);

        // Act
        await Repository.AddAsync(setting);

        // Assert
        ParameterFactory
            .Received()
            .Create(
                Arg.Is("$value"),
                Arg.Is(value));
    }

    [TestCaseSource(nameof(Get_CommandText_KeyAndLabelAndMoment_TestCases))]
    public async Task Get_CommandText_KeyAndLabelAndMoment(string key, string label, DateTimeOffset? moment, string expected)
    {
        // Act
        await Repository.Get(key, label, moment).ToListAsync();

        // Assert
        CommandFactory
            .Received()
            .Create(
                Arg.Any<DbConnection>(),
                Arg.Is($"SELECT etag, key, label, content_type, value, last_modified, locked, tags{expected}"),
                Arg.Any<IEnumerable<DbParameter>?>());
    }

    // ReSharper disable once InconsistentNaming
    private static object[] Get_CommandText_KeyAndLabelAndMoment_TestCases =
    [
        new object?[]
        {
            KeyFilter.Any, LabelFilter.Any, null,
            " FROM configuration_settings"
        },
        new object?[]
        {
            "TestKey", LabelFilter.Any, null,
            " FROM configuration_settings WHERE (key = $key0)"
        },
        new object?[]
        {
            "TestKey*", LabelFilter.Any, null,
            " FROM configuration_settings WHERE (key LIKE $key0)"
        },
        new object?[]
        {
            "TestKey,TestKey*", LabelFilter.Any, null,
            " FROM configuration_settings WHERE (key = $key0 OR key LIKE $key1)"
        },
        new object?[]
        {
            KeyFilter.Any, LabelFilter.Null, null,
            " FROM configuration_settings WHERE (label IS NULL)"
        },
        new object?[]
        {
            KeyFilter.Any, "TestLabel", null,
            " FROM configuration_settings WHERE (label = $label0)"
        },
        new object?[]
        {
            KeyFilter.Any, "TestLabel*", null,
            " FROM configuration_settings WHERE (label LIKE $label0)"
        },
        new object?[]
        {
            KeyFilter.Any, $"{LabelFilter.Null},TestLabel,TestLabel*", null,
            " FROM configuration_settings WHERE (label IS NULL OR label = $label1 OR label LIKE $label2)"
        },
        new object?[]
        {
            KeyFilter.Any, LabelFilter.Any, DateTimeOffset.Parse("2023-10-01T00:00:00+00:00"),
            " FROM configuration_settings_history WHERE (valid_from <= $moment AND valid_to > $moment)"
        },
        new object?[]
        {
            "TestKey", LabelFilter.Null, DateTimeOffset.Parse("2023-10-01T00:00:00+00:00"),
            " FROM configuration_settings_history WHERE (key = $key0) AND (label IS NULL) AND (valid_from <= $moment AND valid_to > $moment)"
        }
    ];

    [TestCase("TestKey", "TestLabel", " WHERE key = $key AND label = $label")]
    [TestCase("TestKey", null, " WHERE key = $key AND label IS NULL")]
    public async Task RemoveAsync_CommandText_ConfigurationSetting(string key, string? label, string expected)
    {
        // Arrange
        var setting = new ConfigurationSetting("TestEtag", key, label, null, null, DateTimeOffset.UtcNow, false, null);

        // Act
        await Repository.RemoveAsync(setting);

        // Assert
        CommandFactory
            .Received()
            .Create(
                Arg.Any<DbConnection>(),
                Arg.Is($"DELETE FROM configuration_settings{expected}"),
                Arg.Any<IEnumerable<DbParameter>?>());
    }

    [Test]
    public async Task RemoveAsync_KeyParameter_ConfigurationSetting()
    {
        // Arrange
        const string key = "TestKey";
        var setting = new ConfigurationSetting("TestEtag", key, null, null, null, DateTimeOffset.UtcNow, false, null);

        // Act
        await Repository.RemoveAsync(setting);

        // Assert
        ParameterFactory
            .Received()
            .Create(
                Arg.Is("$key"),
                Arg.Is(key));
    }

    [TestCase("TestLabel")]
    [TestCase(null)]
    public async Task RemoveAsync_LabelParameter_ConfigurationSetting(string? label)
    {
        // Arrange
        var setting = new ConfigurationSetting("TestEtag", "TestKey", label, null, null, DateTimeOffset.UtcNow, false, null);

        // Act
        await Repository.RemoveAsync(setting);

        // Assert
        if (label is not null)
        {
            ParameterFactory
                .Received()
                .Create(
                    Arg.Is("$label"),
                    Arg.Is(label));
        }
        else
        {
            ParameterFactory
                .DidNotReceive()
                .Create(
                    Arg.Is("$label"),
                    Arg.Any<object?>());
        }
    }

    [TestCase("TestKey", "TestLabel", " WHERE key = $key AND label = $label")]
    [TestCase("TestKey", null, " WHERE key = $key AND label IS NULL")]
    public async Task UpdateAsync_CommandText_ConfigurationSetting(string key, string? label, string expected)
    {
        // Arrange
        var setting = new ConfigurationSetting("TestEtag", key, label, null, null, DateTimeOffset.UtcNow, false, null);

        // Act
        await Repository.UpdateAsync(setting);

        // Assert
        CommandFactory
            .Received()
            .Create(
                Arg.Any<DbConnection>(),
                Arg.Is($"UPDATE configuration_settings SET etag = $etag, content_type = $content_type, value = $value, last_modified = $last_modified, locked = $locked, tags = $tags{expected}"),
                Arg.Any<IEnumerable<DbParameter>?>());
    }

    [Test]
    public async Task UpdateAsync_ContentTypeParameter_ConfigurationSetting()
    {
        // Arrange
        const string contentType = "TestContentType";
        var setting = new ConfigurationSetting("TestEtag", "TestKey", null, contentType, null, DateTimeOffset.UtcNow, false, null);

        // Act
        await Repository.UpdateAsync(setting);

        // Assert
        ParameterFactory
            .Received()
            .Create(
                Arg.Is("$content_type"),
                Arg.Is(contentType));
    }

    [Test]
    public async Task UpdateAsync_EtagParameter_ConfigurationSetting()
    {
        // Arrange
        const string etag = "TestEtag";
        var setting = new ConfigurationSetting(etag, "TestKey", null, null, null, DateTimeOffset.UtcNow, false, null);

        // Act
        await Repository.UpdateAsync(setting);

        // Assert
        ParameterFactory
            .Received()
            .Create(
                Arg.Is("$etag"),
                Arg.Is(etag));
    }

    [Test]
    public async Task UpdateAsync_KeyParameter_ConfigurationSetting()
    {
        // Arrange
        const string key = "TestKey";
        var setting = new ConfigurationSetting("TestEtag", key, null, null, null, DateTimeOffset.UtcNow, false, null);

        // Act
        await Repository.UpdateAsync(setting);

        // Assert
        ParameterFactory
            .Received()
            .Create(
                Arg.Is("$key"),
                Arg.Is(key));
    }

    [TestCase("TestLabel")]
    [TestCase(null)]
    public async Task UpdateAsync_LabelParameter_ConfigurationSetting(string? label)
    {
        // Arrange
        var setting = new ConfigurationSetting("TestEtag", "TestKey", label, null, null, DateTimeOffset.UtcNow, false, null);

        // Act
        await Repository.UpdateAsync(setting);

        // Assert
        if (label is not null)
        {
            ParameterFactory
                .Received()
                .Create(
                    Arg.Is("$label"),
                    Arg.Is(label));
        }
        else
        {
            ParameterFactory
                .DidNotReceive()
                .Create(
                    Arg.Is("$label"),
                    Arg.Any<object?>());
        }
    }

    [Test]
    public async Task UpdateAsync_LastModifiedParameter_ConfigurationSetting()
    {
        // Arrange
        var lastModified = DateTimeOffset.UtcNow;
        var setting = new ConfigurationSetting("TestEtag", "TestKey", null, null, null, lastModified, false, null);

        // Act
        await Repository.UpdateAsync(setting);

        // Assert
        ParameterFactory
            .Received()
            .Create(
                Arg.Is("$last_modified"),
                Arg.Is(lastModified));
    }

    [Test]
    public async Task UpdateAsync_LockedParameter_ConfigurationSetting()
    {
        // Arrange
        const bool locked = false;
        var setting = new ConfigurationSetting("TestEtag", "TestKey", null, null, null, DateTimeOffset.UtcNow, locked, null);

        // Act
        await Repository.UpdateAsync(setting);

        // Assert
        ParameterFactory
            .Received()
            .Create(
                Arg.Is("$locked"),
                Arg.Is(locked));
    }

    [Test]
    public async Task UpdateAsync_TagsParameter_ConfigurationSetting()
    {
        // Arrange
        var tags = new Dictionary<string, object?> { { "TestKey", "TestValue" } };
        var setting = new ConfigurationSetting("TestEtag", "TestKey", null, null, null, DateTimeOffset.UtcNow, false, tags);

        // Act
        await Repository.UpdateAsync(setting);

        // Assert
        ParameterFactory
            .Received()
            .Create(
                Arg.Is("$tags"),
                Arg.Is(tags));
    }

    [Test]
    public async Task UpdateAsync_ValueParameter_ConfigurationSetting()
    {
        // Arrange
        const string value = "TestValue";
        var setting = new ConfigurationSetting("TestEtag", "TestKey", null, null, value, DateTimeOffset.UtcNow, false, null);

        // Act
        await Repository.UpdateAsync(setting);

        // Assert
        ParameterFactory
            .Received()
            .Create(
                Arg.Is("$value"),
                Arg.Is(value));
    }
}
