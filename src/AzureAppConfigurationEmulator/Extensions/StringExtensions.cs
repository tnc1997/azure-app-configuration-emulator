using System.Text;
using System.Web;
using AzureAppConfigurationEmulator.Constants;

namespace AzureAppConfigurationEmulator.Extensions;

public static class StringExtensions
{
    public static string? NormalizeNull(this string s)
    {
        return s is LabelFilter.Null ? null : s;
    }

    public static string Unescape(this string s)
    {
        var builder = new StringBuilder();

        for (int i = 0; i < s.Length; i++)
        {
            if (s[i] == '\\' && i < s.Length - 1)
            {
                i++;
            }

            builder.Append(s[i]);
        }

        return builder.ToString();
    }
}
