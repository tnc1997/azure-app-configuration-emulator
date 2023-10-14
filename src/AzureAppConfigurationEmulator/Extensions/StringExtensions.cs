using AzureAppConfigurationEmulator.Constants;

namespace AzureAppConfigurationEmulator.Extensions;

public static class StringExtensions
{
    public static string? NormalizeNull(this string s)
    {
        return s is LabelFilter.Null ? null : s;
    }
}
