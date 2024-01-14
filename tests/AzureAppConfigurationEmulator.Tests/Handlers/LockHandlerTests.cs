using AzureAppConfigurationEmulator.Constants;
using AzureAppConfigurationEmulator.Entities;
using AzureAppConfigurationEmulator.Handlers;
using AzureAppConfigurationEmulator.Repositories;
using AzureAppConfigurationEmulator.Results;
using Microsoft.AspNetCore.Http.HttpResults;
using NSubstitute;
using NUnit.Framework;

namespace AzureAppConfigurationEmulator.Tests.Handlers;

public class LockHandlerTests
{
    [Test]
    public async Task Lock_KeyValueResult_ExistingConfigurationSetting()
    {
        // Arrange
        var repository = Substitute.For<IConfigurationSettingRepository>();
        var settings = new List<ConfigurationSetting>
        {
            new("abc", "HelloWorld", LabelFilter.Null, null, null, DateTimeOffset.UtcNow, false, null)
        };
        repository.Get(Arg.Any<string>(), Arg.Any<string>()).Returns(settings.ToAsyncEnumerable());

        // Act
        var results = await LockHandler.Lock(repository, "HelloWorld");

        // Assert
        Assert.That(results.Result, Is.TypeOf<KeyValueResult>());
    }

    [TestCase("abc")]
    [TestCase("*")]
    public async Task Lock_KeyValueResult_MatchingIfMatch(string ifMatch)
    {
        // Arrange
        var repository = Substitute.For<IConfigurationSettingRepository>();
        var settings = new List<ConfigurationSetting>
        {
            new("abc", "HelloWorld", LabelFilter.Null, null, null, DateTimeOffset.UtcNow, false, null)
        };
        repository.Get(Arg.Any<string>(), Arg.Any<string>()).Returns(settings.ToAsyncEnumerable());

        // Act
        var results = await LockHandler.Lock(repository, "HelloWorld", ifMatch: ifMatch);

        // Assert
        Assert.That(results.Result, Is.TypeOf<KeyValueResult>());
    }

    [Test]
    public async Task Lock_KeyValueResult_NonMatchingIfNoneMatch()
    {
        // Arrange
        var repository = Substitute.For<IConfigurationSettingRepository>();
        var settings = new List<ConfigurationSetting>
        {
            new("abc", "HelloWorld", LabelFilter.Null, null, null, DateTimeOffset.UtcNow, false, null)
        };
        repository.Get(Arg.Any<string>(), Arg.Any<string>()).Returns(settings.ToAsyncEnumerable());

        // Act
        var results = await LockHandler.Lock(repository, "HelloWorld", ifNoneMatch: "def");

        // Assert
        Assert.That(results.Result, Is.TypeOf<KeyValueResult>());
    }

    [Test]
    public async Task Lock_NotFoundResult_NonExistingConfigurationSetting()
    {
        // Arrange
        var repository = Substitute.For<IConfigurationSettingRepository>();
        var settings = Enumerable.Empty<ConfigurationSetting>();
        repository.Get(Arg.Any<string>(), Arg.Any<string>()).Returns(settings.ToAsyncEnumerable());

        // Act
        var results = await LockHandler.Lock(repository, "HelloWorld");

        // Assert
        Assert.That(results.Result, Is.TypeOf<NotFound>());
    }

    [TestCase("abc")]
    [TestCase("*")]
    public async Task Lock_PreconditionFailedResult_MatchingIfNoneMatch(string ifNoneMatch)
    {
        // Arrange
        var repository = Substitute.For<IConfigurationSettingRepository>();
        var settings = new List<ConfigurationSetting>
        {
            new("abc", "HelloWorld", LabelFilter.Null, null, null, DateTimeOffset.UtcNow, false, null)
        };
        repository.Get(Arg.Any<string>(), Arg.Any<string>()).Returns(settings.ToAsyncEnumerable());

        // Act
        var results = await LockHandler.Lock(repository, "HelloWorld", ifNoneMatch: ifNoneMatch);

        // Assert
        Assert.That(results.Result, Is.TypeOf<PreconditionFailedResult>());
    }

    [Test]
    public async Task Lock_PreconditionFailedResult_NonMatchingIfMatch()
    {
        // Arrange
        var repository = Substitute.For<IConfigurationSettingRepository>();
        var settings = new List<ConfigurationSetting>
        {
            new("abc", "HelloWorld", LabelFilter.Null, null, null, DateTimeOffset.UtcNow, false, null)
        };
        repository.Get(Arg.Any<string>(), Arg.Any<string>()).Returns(settings.ToAsyncEnumerable());

        // Act
        var results = await LockHandler.Lock(repository, "HelloWorld", ifMatch: "def");

        // Assert
        Assert.That(results.Result, Is.TypeOf<PreconditionFailedResult>());
    }

    [Test]
    public async Task Unlock_KeyValueResult_ExistingConfigurationSetting()
    {
        // Arrange
        var repository = Substitute.For<IConfigurationSettingRepository>();
        var settings = new List<ConfigurationSetting>
        {
            new("abc", "HelloWorld", LabelFilter.Null, null, null, DateTimeOffset.UtcNow, false, null)
        };
        repository.Get(Arg.Any<string>(), Arg.Any<string>()).Returns(settings.ToAsyncEnumerable());

        // Act
        var results = await LockHandler.Unlock(repository, "HelloWorld");

        // Assert
        Assert.That(results.Result, Is.TypeOf<KeyValueResult>());
    }

    [TestCase("abc")]
    [TestCase("*")]
    public async Task Unlock_KeyValueResult_MatchingIfMatch(string ifMatch)
    {
        // Arrange
        var repository = Substitute.For<IConfigurationSettingRepository>();
        var settings = new List<ConfigurationSetting>
        {
            new("abc", "HelloWorld", LabelFilter.Null, null, null, DateTimeOffset.UtcNow, false, null)
        };
        repository.Get(Arg.Any<string>(), Arg.Any<string>()).Returns(settings.ToAsyncEnumerable());

        // Act
        var results = await LockHandler.Unlock(repository, "HelloWorld", ifMatch: ifMatch);

        // Assert
        Assert.That(results.Result, Is.TypeOf<KeyValueResult>());
    }

    [Test]
    public async Task Unlock_KeyValueResult_NonMatchingIfNoneMatch()
    {
        // Arrange
        var repository = Substitute.For<IConfigurationSettingRepository>();
        var settings = new List<ConfigurationSetting>
        {
            new("abc", "HelloWorld", LabelFilter.Null, null, null, DateTimeOffset.UtcNow, false, null)
        };
        repository.Get(Arg.Any<string>(), Arg.Any<string>()).Returns(settings.ToAsyncEnumerable());

        // Act
        var results = await LockHandler.Unlock(repository, "HelloWorld", ifNoneMatch: "def");

        // Assert
        Assert.That(results.Result, Is.TypeOf<KeyValueResult>());
    }

    [Test]
    public async Task Unlock_NotFoundResult_NonExistingConfigurationSetting()
    {
        // Arrange
        var repository = Substitute.For<IConfigurationSettingRepository>();
        var settings = Enumerable.Empty<ConfigurationSetting>();
        repository.Get(Arg.Any<string>(), Arg.Any<string>()).Returns(settings.ToAsyncEnumerable());

        // Act
        var results = await LockHandler.Unlock(repository, "HelloWorld");

        // Assert
        Assert.That(results.Result, Is.TypeOf<NotFound>());
    }

    [TestCase("abc")]
    [TestCase("*")]
    public async Task Unlock_PreconditionFailedResult_MatchingIfNoneMatch(string ifNoneMatch)
    {
        // Arrange
        var repository = Substitute.For<IConfigurationSettingRepository>();
        var settings = new List<ConfigurationSetting>
        {
            new("abc", "HelloWorld", LabelFilter.Null, null, null, DateTimeOffset.UtcNow, false, null)
        };
        repository.Get(Arg.Any<string>(), Arg.Any<string>()).Returns(settings.ToAsyncEnumerable());

        // Act
        var results = await LockHandler.Unlock(repository, "HelloWorld", ifNoneMatch: ifNoneMatch);

        // Assert
        Assert.That(results.Result, Is.TypeOf<PreconditionFailedResult>());
    }

    [Test]
    public async Task Unlock_PreconditionFailedResult_NonMatchingIfMatch()
    {
        // Arrange
        var repository = Substitute.For<IConfigurationSettingRepository>();
        var settings = new List<ConfigurationSetting>
        {
            new("abc", "HelloWorld", LabelFilter.Null, null, null, DateTimeOffset.UtcNow, false, null)
        };
        repository.Get(Arg.Any<string>(), Arg.Any<string>()).Returns(settings.ToAsyncEnumerable());

        // Act
        var results = await LockHandler.Unlock(repository, "HelloWorld", ifMatch: "def");

        // Assert
        Assert.That(results.Result, Is.TypeOf<PreconditionFailedResult>());
    }
}
