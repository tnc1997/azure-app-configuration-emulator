using AzureAppConfigurationEmulator.Entities;
using AzureAppConfigurationEmulator.Handlers;
using AzureAppConfigurationEmulator.Repositories;
using AzureAppConfigurationEmulator.Results;
using Microsoft.AspNetCore.Http.HttpResults;
using NSubstitute;
using NUnit.Framework;

namespace AzureAppConfigurationEmulator.Tests.Handlers;

public class KeyValueHandlerTests
{
    [Test]
    public async Task Delete_KeyValueResult_ExistingConfigurationSetting()
    {
        // Arrange
        var repository = Substitute.For<IConfigurationSettingRepository>();
        var settings = new List<ConfigurationSetting>
        {
            new("TestEtag", "TestKey", null, null, null, DateTimeOffset.UtcNow, false, null)
        };
        repository.Get(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<DateTimeOffset?>(), Arg.Any<CancellationToken>()).Returns(settings.ToAsyncEnumerable());

        // Act
        var results = await KeyValueHandler.Delete(repository, "HelloWorld");

        // Assert
        Assert.That(results.Result, Is.TypeOf<KeyValueResult>());
    }

    [TestCase("TestEtag")]
    [TestCase("*")]
    public async Task Delete_KeyValueResult_ExistingConfigurationSettingMatchingIfMatch(string ifMatch)
    {
        // Arrange
        var repository = Substitute.For<IConfigurationSettingRepository>();
        var settings = new List<ConfigurationSetting>
        {
            new("TestEtag", "TestKey", null, null, null, DateTimeOffset.UtcNow, false, null)
        };
        repository.Get(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<DateTimeOffset?>(), Arg.Any<CancellationToken>()).Returns(settings.ToAsyncEnumerable());

        // Act
        var results = await KeyValueHandler.Delete(repository, "TestKey", ifMatch: ifMatch);

        // Assert
        Assert.That(results.Result, Is.TypeOf<KeyValueResult>());
    }

    [Test]
    public async Task Delete_KeyValueResult_ExistingConfigurationSettingNonMatchingIfNoneMatch()
    {
        // Arrange
        var repository = Substitute.For<IConfigurationSettingRepository>();
        var settings = new List<ConfigurationSetting>
        {
            new("TestEtag", "TestKey", null, null, null, DateTimeOffset.UtcNow, false, null)
        };
        repository.Get(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<DateTimeOffset?>(), Arg.Any<CancellationToken>()).Returns(settings.ToAsyncEnumerable());

        // Act
        var results = await KeyValueHandler.Delete(repository, "TestKey", ifNoneMatch: "abc");

        // Assert
        Assert.That(results.Result, Is.TypeOf<KeyValueResult>());
    }

    [Test]
    public async Task Delete_NoContentResult_NonExistingConfigurationSetting()
    {
        // Arrange
        var repository = Substitute.For<IConfigurationSettingRepository>();
        var settings = Enumerable.Empty<ConfigurationSetting>();
        repository.Get(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<DateTimeOffset?>(), Arg.Any<CancellationToken>()).Returns(settings.ToAsyncEnumerable());

        // Act
        var results = await KeyValueHandler.Delete(repository, "TestKey");

        // Assert
        Assert.That(results.Result, Is.TypeOf<NoContent>());
    }

    [Test]
    public async Task Delete_NoContentResult_NonExistingConfigurationSettingMatchingIfNoneMatch()
    {
        // Arrange
        var repository = Substitute.For<IConfigurationSettingRepository>();
        var settings = Enumerable.Empty<ConfigurationSetting>();
        repository.Get(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<DateTimeOffset?>(), Arg.Any<CancellationToken>()).Returns(settings.ToAsyncEnumerable());

        // Act
        var results = await KeyValueHandler.Delete(repository, "TestKey", ifNoneMatch: "*");

        // Assert
        Assert.That(results.Result, Is.TypeOf<NoContent>());
    }

    [Test]
    public async Task Delete_NoContentResult_NonExistingConfigurationSettingNonMatchingIfMatch()
    {
        // Arrange
        var repository = Substitute.For<IConfigurationSettingRepository>();
        var settings = Enumerable.Empty<ConfigurationSetting>();
        repository.Get(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<DateTimeOffset?>(), Arg.Any<CancellationToken>()).Returns(settings.ToAsyncEnumerable());

        // Act
        var results = await KeyValueHandler.Delete(repository, "TestKey", ifMatch: "TestEtag");

        // Assert
        Assert.That(results.Result, Is.TypeOf<NoContent>());
    }

    [TestCase("TestEtag")]
    [TestCase("*")]
    public async Task Delete_PreconditionFailedResult_ExistingConfigurationSettingMatchingIfNoneMatch(string ifNoneMatch)
    {
        // Arrange
        var repository = Substitute.For<IConfigurationSettingRepository>();
        var settings = new List<ConfigurationSetting>
        {
            new("TestEtag", "TestKey", null, null, null, DateTimeOffset.UtcNow, false, null)
        };
        repository.Get(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<DateTimeOffset?>(), Arg.Any<CancellationToken>()).Returns(settings.ToAsyncEnumerable());

        // Act
        var results = await KeyValueHandler.Delete(repository, "TestKey", ifNoneMatch: ifNoneMatch);

        // Assert
        Assert.That(results.Result, Is.TypeOf<PreconditionFailedResult>());
    }

    [Test]
    public async Task Delete_PreconditionFailedResult_ExistingConfigurationSettingNonMatchingIfMatch()
    {
        // Arrange
        var repository = Substitute.For<IConfigurationSettingRepository>();
        var settings = new List<ConfigurationSetting>
        {
            new("TestEtag", "TestKey", null, null, null, DateTimeOffset.UtcNow, false, null)
        };
        repository.Get(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<DateTimeOffset?>(), Arg.Any<CancellationToken>()).Returns(settings.ToAsyncEnumerable());

        // Act
        var results = await KeyValueHandler.Delete(repository, "TestKey", ifMatch: "abc");

        // Assert
        Assert.That(results.Result, Is.TypeOf<PreconditionFailedResult>());
    }

    [Test]
    public async Task Delete_PreconditionFailedResult_NonExistingConfigurationSettingMatchingIfNoneMatch()
    {
        // Arrange
        var repository = Substitute.For<IConfigurationSettingRepository>();
        var settings = Enumerable.Empty<ConfigurationSetting>();
        repository.Get(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<DateTimeOffset?>(), Arg.Any<CancellationToken>()).Returns(settings.ToAsyncEnumerable());

        // Act
        var results = await KeyValueHandler.Delete(repository, "TestKey", ifNoneMatch: "TestEtag");

        // Assert
        Assert.That(results.Result, Is.TypeOf<PreconditionFailedResult>());
    }

    [Test]
    public async Task Delete_PreconditionFailedResult_NonExistingConfigurationSettingNonMatchingIfMatch()
    {
        // Arrange
        var repository = Substitute.For<IConfigurationSettingRepository>();
        var settings = Enumerable.Empty<ConfigurationSetting>();
        repository.Get(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<DateTimeOffset?>(), Arg.Any<CancellationToken>()).Returns(settings.ToAsyncEnumerable());

        // Act
        var results = await KeyValueHandler.Delete(repository, "TestKey", ifMatch: "*");

        // Assert
        Assert.That(results.Result, Is.TypeOf<PreconditionFailedResult>());
    }

    [Test]
    public async Task Delete_ReadOnlyResult_LockedConfigurationSetting()
    {
        // Arrange
        var repository = Substitute.For<IConfigurationSettingRepository>();
        var settings = new List<ConfigurationSetting>
        {
            new("TestEtag", "TestKey", null, null, null, DateTimeOffset.UtcNow, true, null)
        };
        repository.Get(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<DateTimeOffset?>(), Arg.Any<CancellationToken>()).Returns(settings.ToAsyncEnumerable());

        // Act
        var results = await KeyValueHandler.Delete(repository, "TestKey");

        // Assert
        Assert.That(results.Result, Is.TypeOf<ReadOnlyResult>());
    }

    [Test]
    public async Task Get_KeyValueResult_ExistingConfigurationSetting()
    {
        // Arrange
        var repository = Substitute.For<IConfigurationSettingRepository>();
        var settings = new List<ConfigurationSetting>
        {
            new("TestEtag", "TestKey", null, null, null, DateTimeOffset.UtcNow, false, null)
        };
        repository.Get(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<DateTimeOffset?>(), Arg.Any<CancellationToken>()).Returns(settings.ToAsyncEnumerable());

        // Act
        var results = await KeyValueHandler.Get(repository, "TestKey");

        // Assert
        Assert.That(results.Result, Is.TypeOf<KeyValueResult>());
    }

    [TestCase("TestEtag")]
    [TestCase("*")]
    public async Task Get_KeyValueResult_ExistingConfigurationSettingMatchingIfMatch(string ifMatch)
    {
        // Arrange
        var repository = Substitute.For<IConfigurationSettingRepository>();
        var settings = new List<ConfigurationSetting>
        {
            new("TestEtag", "TestKey", null, null, null, DateTimeOffset.UtcNow, false, null)
        };
        repository.Get(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<DateTimeOffset?>(), Arg.Any<CancellationToken>()).Returns(settings.ToAsyncEnumerable());

        // Act
        var results = await KeyValueHandler.Get(repository, "TestKey", ifMatch: ifMatch);

        // Assert
        Assert.That(results.Result, Is.TypeOf<KeyValueResult>());
    }

    [Test]
    public async Task Get_KeyValueResult_ExistingConfigurationSettingNonMatchingIfNoneMatch()
    {
        // Arrange
        var repository = Substitute.For<IConfigurationSettingRepository>();
        var settings = new List<ConfigurationSetting>
        {
            new("TestEtag", "TestKey", null, null, null, DateTimeOffset.UtcNow, false, null)
        };
        repository.Get(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<DateTimeOffset?>(), Arg.Any<CancellationToken>()).Returns(settings.ToAsyncEnumerable());

        // Act
        var results = await KeyValueHandler.Get(repository, "TestKey", ifNoneMatch: "abc");

        // Assert
        Assert.That(results.Result, Is.TypeOf<KeyValueResult>());
    }

    [Test]
    public async Task Get_NotFoundResult_NonExistingConfigurationSetting()
    {
        // Arrange
        var repository = Substitute.For<IConfigurationSettingRepository>();
        var settings = Enumerable.Empty<ConfigurationSetting>();
        repository.Get(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<DateTimeOffset?>(), Arg.Any<CancellationToken>()).Returns(settings.ToAsyncEnumerable());

        // Act
        var results = await KeyValueHandler.Get(repository, "TestKey");

        // Assert
        Assert.That(results.Result, Is.TypeOf<NotFound>());
    }

    [Test]
    public async Task Get_NotFoundResult_NonExistingConfigurationSettingMatchingIfNoneMatch()
    {
        // Arrange
        var repository = Substitute.For<IConfigurationSettingRepository>();
        var settings = Enumerable.Empty<ConfigurationSetting>();
        repository.Get(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<DateTimeOffset?>(), Arg.Any<CancellationToken>()).Returns(settings.ToAsyncEnumerable());

        // Act
        var results = await KeyValueHandler.Get(repository, "TestKey", ifNoneMatch: "TestEtag");

        // Assert
        Assert.That(results.Result, Is.TypeOf<NotFound>());
    }

    [Test]
    public async Task Get_NotFoundResult_NonExistingConfigurationSettingNonMatchingIfMatch()
    {
        // Arrange
        var repository = Substitute.For<IConfigurationSettingRepository>();
        var settings = Enumerable.Empty<ConfigurationSetting>();
        repository.Get(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<DateTimeOffset?>(), Arg.Any<CancellationToken>()).Returns(settings.ToAsyncEnumerable());

        // Act
        var results = await KeyValueHandler.Get(repository, "TestKey", ifMatch: "*");

        // Assert
        Assert.That(results.Result, Is.TypeOf<NotFound>());
    }

    [TestCase("TestEtag")]
    [TestCase("*")]
    public async Task Get_NotModifiedResult_ExistingConfigurationSettingMatchingIfNoneMatch(string ifNoneMatch)
    {
        // Arrange
        var repository = Substitute.For<IConfigurationSettingRepository>();
        var settings = new List<ConfigurationSetting>
        {
            new("TestEtag", "TestKey", null, null, null, DateTimeOffset.UtcNow, false, null)
        };
        repository.Get(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<DateTimeOffset?>(), Arg.Any<CancellationToken>()).Returns(settings.ToAsyncEnumerable());

        // Act
        var results = await KeyValueHandler.Get(repository, "TestKey", ifNoneMatch: ifNoneMatch);

        // Assert
        Assert.That(results.Result, Is.TypeOf<NotModifiedResult>());
    }

    [Test]
    public async Task Get_PreconditionFailedResult_ExistingConfigurationSettingNonMatchingIfMatch()
    {
        // Arrange
        var repository = Substitute.For<IConfigurationSettingRepository>();
        var settings = new List<ConfigurationSetting>
        {
            new("TestEtag", "TestKey", null, null, null, DateTimeOffset.UtcNow, false, null)
        };
        repository.Get(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<DateTimeOffset?>(), Arg.Any<CancellationToken>()).Returns(settings.ToAsyncEnumerable());

        // Act
        var results = await KeyValueHandler.Get(repository, "TestKey", ifMatch: "abc");

        // Assert
        Assert.That(results.Result, Is.TypeOf<PreconditionFailedResult>());
    }

    [Test]
    public async Task Set_KeyValueResult_ExistingConfigurationSetting()
    {
        // Arrange
        var repository = Substitute.For<IConfigurationSettingRepository>();
        var settings = new List<ConfigurationSetting>
        {
            new("TestEtag", "TestKey", null, null, null, DateTimeOffset.UtcNow, false, null)
        };
        repository.Get(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<DateTimeOffset?>(), Arg.Any<CancellationToken>()).Returns(settings.ToAsyncEnumerable());

        // Act
        var input = new KeyValueHandler.SetInput(null, null, null);
        var results = await KeyValueHandler.Set(repository, input, "TestKey");

        // Assert
        Assert.That(results.Result, Is.TypeOf<KeyValueResult>());
    }

    [TestCase("TestEtag")]
    [TestCase("*")]
    public async Task Set_KeyValueResult_ExistingConfigurationSettingMatchingIfMatch(string ifMatch)
    {
        // Arrange
        var repository = Substitute.For<IConfigurationSettingRepository>();
        var settings = new List<ConfigurationSetting>
        {
            new("TestEtag", "TestKey", null, null, null, DateTimeOffset.UtcNow, false, null)
        };
        repository.Get(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<DateTimeOffset?>(), Arg.Any<CancellationToken>()).Returns(settings.ToAsyncEnumerable());

        // Act
        var input = new KeyValueHandler.SetInput(null, null, null);
        var results = await KeyValueHandler.Set(repository, input, "TestKey", ifMatch: ifMatch);

        // Assert
        Assert.That(results.Result, Is.TypeOf<KeyValueResult>());
    }

    [Test]
    public async Task Set_KeyValueResult_ExistingConfigurationSettingNonMatchingIfNoneMatch()
    {
        // Arrange
        var repository = Substitute.For<IConfigurationSettingRepository>();
        var settings = new List<ConfigurationSetting>
        {
            new("TestEtag", "TestKey", null, null, null, DateTimeOffset.UtcNow, false, null)
        };
        repository.Get(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<DateTimeOffset?>(), Arg.Any<CancellationToken>()).Returns(settings.ToAsyncEnumerable());

        // Act
        var input = new KeyValueHandler.SetInput(null, null, null);
        var results = await KeyValueHandler.Set(repository, input, "TestKey", ifNoneMatch: "abc");

        // Assert
        Assert.That(results.Result, Is.TypeOf<KeyValueResult>());
    }

    [Test]
    public async Task Set_KeyValueResult_NonExistingConfigurationSetting()
    {
        // Arrange
        var repository = Substitute.For<IConfigurationSettingRepository>();
        var settings = Enumerable.Empty<ConfigurationSetting>();
        repository.Get(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<DateTimeOffset?>(), Arg.Any<CancellationToken>()).Returns(settings.ToAsyncEnumerable());

        // Act
        var input = new KeyValueHandler.SetInput(null, null, null);
        var results = await KeyValueHandler.Set(repository, input, "TestKey");

        // Assert
        Assert.That(results.Result, Is.TypeOf<KeyValueResult>());
    }

    [Test]
    public async Task Set_KeyValueResult_NonExistingConfigurationSettingMatchingIfNoneMatch()
    {
        // Arrange
        var repository = Substitute.For<IConfigurationSettingRepository>();
        var settings = Enumerable.Empty<ConfigurationSetting>();
        repository.Get(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<DateTimeOffset?>(), Arg.Any<CancellationToken>()).Returns(settings.ToAsyncEnumerable());

        // Act
        var input = new KeyValueHandler.SetInput(null, null, null);
        var results = await KeyValueHandler.Set(repository, input, "TestKey", ifNoneMatch: "*");

        // Assert
        Assert.That(results.Result, Is.TypeOf<KeyValueResult>());
    }

    [Test]
    public async Task Set_KeyValueResult_NonExistingConfigurationSettingNonMatchingIfMatch()
    {
        // Arrange
        var repository = Substitute.For<IConfigurationSettingRepository>();
        var settings = Enumerable.Empty<ConfigurationSetting>();
        repository.Get(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<DateTimeOffset?>(), Arg.Any<CancellationToken>()).Returns(settings.ToAsyncEnumerable());

        // Act
        var input = new KeyValueHandler.SetInput(null, null, null);
        var results = await KeyValueHandler.Set(repository, input, "TestKey", ifMatch: "TestEtag");

        // Assert
        Assert.That(results.Result, Is.TypeOf<KeyValueResult>());
    }

    [TestCase("TestEtag")]
    [TestCase("*")]
    public async Task Set_PreconditionFailedResult_ExistingConfigurationSettingMatchingIfNoneMatch(string ifNoneMatch)
    {
        // Arrange
        var repository = Substitute.For<IConfigurationSettingRepository>();
        var settings = new List<ConfigurationSetting>
        {
            new("TestEtag", "TestKey", null, null, null, DateTimeOffset.UtcNow, false, null)
        };
        repository.Get(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<DateTimeOffset?>(), Arg.Any<CancellationToken>()).Returns(settings.ToAsyncEnumerable());

        // Act
        var input = new KeyValueHandler.SetInput(null, null, null);
        var results = await KeyValueHandler.Set(repository, input, "TestKey", ifNoneMatch: ifNoneMatch);

        // Assert
        Assert.That(results.Result, Is.TypeOf<PreconditionFailedResult>());
    }

    [Test]
    public async Task Set_PreconditionFailedResult_ExistingConfigurationSettingNonMatchingIfMatch()
    {
        // Arrange
        var repository = Substitute.For<IConfigurationSettingRepository>();
        var settings = new List<ConfigurationSetting>
        {
            new("TestEtag", "TestKey", null, null, null, DateTimeOffset.UtcNow, false, null)
        };
        repository.Get(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<DateTimeOffset?>(), Arg.Any<CancellationToken>()).Returns(settings.ToAsyncEnumerable());

        // Act
        var input = new KeyValueHandler.SetInput(null, null, null);
        var results = await KeyValueHandler.Set(repository, input, "TestKey", ifMatch: "abc");

        // Assert
        Assert.That(results.Result, Is.TypeOf<PreconditionFailedResult>());
    }

    [Test]
    public async Task Set_PreconditionFailedResult_NonExistingConfigurationSettingMatchingIfNoneMatch()
    {
        // Arrange
        var repository = Substitute.For<IConfigurationSettingRepository>();
        var settings = Enumerable.Empty<ConfigurationSetting>();
        repository.Get(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<DateTimeOffset?>(), Arg.Any<CancellationToken>()).Returns(settings.ToAsyncEnumerable());

        // Act
        var input = new KeyValueHandler.SetInput(null, null, null);
        var results = await KeyValueHandler.Set(repository, input, "TestKey", ifNoneMatch: "TestEtag");

        // Assert
        Assert.That(results.Result, Is.TypeOf<PreconditionFailedResult>());
    }

    [Test]
    public async Task Set_PreconditionFailedResult_NonExistingConfigurationSettingNonMatchingIfMatch()
    {
        // Arrange
        var repository = Substitute.For<IConfigurationSettingRepository>();
        var settings = Enumerable.Empty<ConfigurationSetting>();
        repository.Get(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<DateTimeOffset?>(), Arg.Any<CancellationToken>()).Returns(settings.ToAsyncEnumerable());

        // Act
        var input = new KeyValueHandler.SetInput(null, null, null);
        var results = await KeyValueHandler.Set(repository, input, "TestKey", ifMatch: "*");

        // Assert
        Assert.That(results.Result, Is.TypeOf<PreconditionFailedResult>());
    }

    [Test]
    public async Task Set_ReadOnlyResult_LockedConfigurationSetting()
    {
        // Arrange
        var repository = Substitute.For<IConfigurationSettingRepository>();
        var settings = new List<ConfigurationSetting>
        {
            new("TestEtag", "TestKey", null, null, null, DateTimeOffset.UtcNow, true, null)
        };
        repository.Get(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<DateTimeOffset?>(), Arg.Any<CancellationToken>()).Returns(settings.ToAsyncEnumerable());

        // Act
        var input = new KeyValueHandler.SetInput(null, null, null);
        var results = await KeyValueHandler.Set(repository, input, "TestKey");

        // Assert
        Assert.That(results.Result, Is.TypeOf<ReadOnlyResult>());
    }
}
