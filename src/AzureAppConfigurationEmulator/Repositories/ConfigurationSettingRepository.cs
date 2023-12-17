using System.Text.RegularExpressions;
using AzureAppConfigurationEmulator.Constants;
using AzureAppConfigurationEmulator.Contexts;
using AzureAppConfigurationEmulator.Entities;
using AzureAppConfigurationEmulator.Extensions;
using LinqKit;
using Microsoft.EntityFrameworkCore;

namespace AzureAppConfigurationEmulator.Repositories;

public class ConfigurationSettingRepository(ApplicationDbContext context) : IConfigurationSettingRepository
{
    private ApplicationDbContext Context { get; } = context;

    public async Task AddAsync(ConfigurationSetting setting, CancellationToken cancellationToken = default)
    {
        Context.ConfigurationSettings.Add(setting);

        await Context.SaveChangesAsync(cancellationToken);
    }

    public IAsyncEnumerable<ConfigurationSetting> Get(string key = KeyFilter.Any, string label = LabelFilter.Any)
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

        return Context.ConfigurationSettings.Where(outer).AsAsyncEnumerable();
    }

    public async Task RemoveAsync(ConfigurationSetting setting, CancellationToken cancellationToken = default)
    {
        Context.ConfigurationSettings.Remove(setting);

        await Context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(ConfigurationSetting setting, CancellationToken cancellationToken = default)
    {
        Context.ConfigurationSettings.Update(setting);

        await Context.SaveChangesAsync(cancellationToken);
    }
}
