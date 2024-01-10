using AzureAppConfigurationEmulator.Extensions;
using NUnit.Framework;

namespace AzureAppConfigurationEmulator.Tests.Extensions;

public class StringExtensionsTests
{
    [TestCase(".appconfig.featureflag/flag1", ".appconfig.featureflag/flag1")]
    [TestCase(".appconfig.featureflag%2Fflag1", ".appconfig.featureflag/flag1")]
    [TestCase(".appconfig.featureflag%2Fflag1+flag2", ".appconfig.featureflag/flag1+flag2")]
    public void UrlDecodeSlashesTests(string input, string expected)
    {
        var result = input.UrlDecodeSlashes();

        Assert.That(result, Is.EqualTo(expected));
    }
}
