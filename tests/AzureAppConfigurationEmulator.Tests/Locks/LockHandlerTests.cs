using AzureAppConfigurationEmulator.Common;
using AzureAppConfigurationEmulator.ConfigurationSettings;
using AzureAppConfigurationEmulator.Locks;
using Microsoft.AspNetCore.Http.HttpResults;
using NSubstitute;
using NUnit.Framework;

namespace AzureAppConfigurationEmulator.Tests.Locks;

public class LockHandlerTests
{
    private IConfigurationSettingRepository Repository { get; set; }

    [SetUp]
    public void SetUp()
    {
        Repository = Substitute.For<IConfigurationSettingRepository>();
    }

    [Test]
    public async Task Lock_ConfigurationSettingResult_ExistingConfigurationSetting()
    {
        // Arrange
        var settings = new List<ConfigurationSetting>
        {
            new("TestEtag", "TestKey", DateTimeOffset.UtcNow, false)
        };
        Repository.Get(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<DateTimeOffset?>(), Arg.Any<CancellationToken>()).Returns(settings.ToAsyncEnumerable());

        // Act
        var results = await LockHandler.Lock(Repository, "TestKey");

        // Assert
        Assert.That(results.Result, Is.TypeOf<ConfigurationSettingResult>());
    }

    [TestCase("TestEtag")]
    [TestCase("*")]
    public async Task Lock_ConfigurationSettingResult_MatchingIfMatch(string ifMatch)
    {
        // Arrange
        var settings = new List<ConfigurationSetting>
        {
            new("TestEtag", "TestKey", DateTimeOffset.UtcNow, false)
        };
        Repository.Get(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<DateTimeOffset?>(), Arg.Any<CancellationToken>()).Returns(settings.ToAsyncEnumerable());

        // Act
        var results = await LockHandler.Lock(Repository, "TestKey", ifMatch: ifMatch);

        // Assert
        Assert.That(results.Result, Is.TypeOf<ConfigurationSettingResult>());
    }

    [Test]
    public async Task Lock_ConfigurationSettingResult_NonMatchingIfNoneMatch()
    {
        // Arrange
        var settings = new List<ConfigurationSetting>
        {
            new("TestEtag", "TestKey", DateTimeOffset.UtcNow, false)
        };
        Repository.Get(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<DateTimeOffset?>(), Arg.Any<CancellationToken>()).Returns(settings.ToAsyncEnumerable());

        // Act
        var results = await LockHandler.Lock(Repository, "TestKey", ifNoneMatch: "abc");

        // Assert
        Assert.That(results.Result, Is.TypeOf<ConfigurationSettingResult>());
    }

    [Test]
    public async Task Lock_NotFoundResult_NonExistingConfigurationSetting()
    {
        // Arrange
        var settings = Enumerable.Empty<ConfigurationSetting>();
        Repository.Get(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<DateTimeOffset?>(), Arg.Any<CancellationToken>()).Returns(settings.ToAsyncEnumerable());

        // Act
        var results = await LockHandler.Lock(Repository, "TestKey");

        // Assert
        Assert.That(results.Result, Is.TypeOf<NotFound>());
    }

    [TestCase("TestEtag")]
    [TestCase("*")]
    public async Task Lock_PreconditionFailedResult_MatchingIfNoneMatch(string ifNoneMatch)
    {
        // Arrange
        var settings = new List<ConfigurationSetting>
        {
            new("TestEtag", "TestKey", DateTimeOffset.UtcNow, false)
        };
        Repository.Get(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<DateTimeOffset?>(), Arg.Any<CancellationToken>()).Returns(settings.ToAsyncEnumerable());

        // Act
        var results = await LockHandler.Lock(Repository, "TestKey", ifNoneMatch: ifNoneMatch);

        // Assert
        Assert.That(results.Result, Is.TypeOf<PreconditionFailedResult>());
    }

    [Test]
    public async Task Lock_PreconditionFailedResult_NonMatchingIfMatch()
    {
        // Arrange
        var settings = new List<ConfigurationSetting>
        {
            new("TestEtag", "TestKey", DateTimeOffset.UtcNow, false)
        };
        Repository.Get(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<DateTimeOffset?>(), Arg.Any<CancellationToken>()).Returns(settings.ToAsyncEnumerable());

        // Act
        var results = await LockHandler.Lock(Repository, "TestKey", ifMatch: "abc");

        // Assert
        Assert.That(results.Result, Is.TypeOf<PreconditionFailedResult>());
    }

    [Test]
    public async Task Unlock_ConfigurationSettingResult_ExistingConfigurationSetting()
    {
        // Arrange
        var settings = new List<ConfigurationSetting>
        {
            new("TestEtag", "TestKey", DateTimeOffset.UtcNow, false)
        };
        Repository.Get(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<DateTimeOffset?>(), Arg.Any<CancellationToken>()).Returns(settings.ToAsyncEnumerable());

        // Act
        var results = await LockHandler.Unlock(Repository, "TestKey");

        // Assert
        Assert.That(results.Result, Is.TypeOf<ConfigurationSettingResult>());
    }

    [TestCase("TestEtag")]
    [TestCase("*")]
    public async Task Unlock_ConfigurationSettingResult_MatchingIfMatch(string ifMatch)
    {
        // Arrange
        var settings = new List<ConfigurationSetting>
        {
            new("TestEtag", "TestKey", DateTimeOffset.UtcNow, false)
        };
        Repository.Get(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<DateTimeOffset?>(), Arg.Any<CancellationToken>()).Returns(settings.ToAsyncEnumerable());

        // Act
        var results = await LockHandler.Unlock(Repository, "TestKey", ifMatch: ifMatch);

        // Assert
        Assert.That(results.Result, Is.TypeOf<ConfigurationSettingResult>());
    }

    [Test]
    public async Task Unlock_ConfigurationSettingResult_NonMatchingIfNoneMatch()
    {
        // Arrange
        var settings = new List<ConfigurationSetting>
        {
            new("TestEtag", "TestKey", DateTimeOffset.UtcNow, false)
        };
        Repository.Get(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<DateTimeOffset?>(), Arg.Any<CancellationToken>()).Returns(settings.ToAsyncEnumerable());

        // Act
        var results = await LockHandler.Unlock(Repository, "TestKey", ifNoneMatch: "abc");

        // Assert
        Assert.That(results.Result, Is.TypeOf<ConfigurationSettingResult>());
    }

    [Test]
    public async Task Unlock_NotFoundResult_NonExistingConfigurationSetting()
    {
        // Arrange
        var settings = Enumerable.Empty<ConfigurationSetting>();
        Repository.Get(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<DateTimeOffset?>(), Arg.Any<CancellationToken>()).Returns(settings.ToAsyncEnumerable());

        // Act
        var results = await LockHandler.Unlock(Repository, "TestKey");

        // Assert
        Assert.That(results.Result, Is.TypeOf<NotFound>());
    }

    [TestCase("TestEtag")]
    [TestCase("*")]
    public async Task Unlock_PreconditionFailedResult_MatchingIfNoneMatch(string ifNoneMatch)
    {
        // Arrange
        var settings = new List<ConfigurationSetting>
        {
            new("TestEtag", "TestKey", DateTimeOffset.UtcNow, false)
        };
        Repository.Get(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<DateTimeOffset?>(), Arg.Any<CancellationToken>()).Returns(settings.ToAsyncEnumerable());

        // Act
        var results = await LockHandler.Unlock(Repository, "TestKey", ifNoneMatch: ifNoneMatch);

        // Assert
        Assert.That(results.Result, Is.TypeOf<PreconditionFailedResult>());
    }

    [Test]
    public async Task Unlock_PreconditionFailedResult_NonMatchingIfMatch()
    {
        // Arrange
        var settings = new List<ConfigurationSetting>
        {
            new("TestEtag", "TestKey", DateTimeOffset.UtcNow, false)
        };
        Repository.Get(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<DateTimeOffset?>(), Arg.Any<CancellationToken>()).Returns(settings.ToAsyncEnumerable());

        // Act
        var results = await LockHandler.Unlock(Repository, "TestKey", ifMatch: "abc");

        // Assert
        Assert.That(results.Result, Is.TypeOf<PreconditionFailedResult>());
    }
}
