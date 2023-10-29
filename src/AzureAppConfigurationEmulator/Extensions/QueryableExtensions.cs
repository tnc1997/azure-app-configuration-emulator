using System.Text.RegularExpressions;
using AzureAppConfigurationEmulator.Constants;
using AzureAppConfigurationEmulator.Entities;
using LinqKit;

namespace AzureAppConfigurationEmulator.Extensions;

public static class QueryableExtensions
{
    public static IQueryable<ConfigurationSetting> Where(
        this IQueryable<ConfigurationSetting> settings,
        string key = KeyFilter.Any,
        string label = LabelFilter.Any)
    {
        var outer = PredicateBuilder.New<ConfigurationSetting>(true);

        if (key != KeyFilter.Any)
        {
            var inner = PredicateBuilder.New<ConfigurationSetting>(false);

            foreach (var s in new Regex(@"(?<!\\),").Split(key).Select(s => s.Unescape()))
            {
                var match = new Regex(@"^(.*)(?<!\\)\*$").Match(s);

                if (match.Success)
                {
                    inner = inner.Or(setting => setting.Key.StartsWith(match.Groups[1].Value));
                }
                else
                {
                    inner = inner.Or(setting => setting.Key == s);
                }
            }

            outer = outer.And(inner);
        }

        if (label != LabelFilter.Any)
        {
            var inner = PredicateBuilder.New<ConfigurationSetting>(false);

            foreach (var s in new Regex(@"(?<!\\),").Split(label).Select(s => s.Unescape()))
            {
                var match = new Regex(@"^(.*)(?<!\\)\*$").Match(s);

                if (match.Success)
                {
                    inner = inner.Or(setting => setting.Label.StartsWith(match.Groups[1].Value));
                }
                else
                {
                    inner = inner.Or(setting => setting.Label == s);
                }
            }

            outer = outer.And(inner);
        }

        return settings.Where(outer);
    }
}
