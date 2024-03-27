using System.Text;

namespace AzureAppConfigurationEmulator.Common.Extensions;

public static class StringExtensions
{
    public static string Unescape(this string s)
    {
        var builder = new StringBuilder();

        for (int i = 0; i < s.Length; i++)
        {
            if (s[i] is '\\' && i < s.Length - 1)
            {
                i++;
            }

            builder.Append(s[i]);
        }

        return builder.ToString();
    }
}
