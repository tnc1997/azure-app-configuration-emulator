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

        Context.ConfigurationSettingRevisions.Add(new ConfigurationSettingRevision(setting));

        await Context.SaveChangesAsync(cancellationToken);
    }

    public IAsyncEnumerable<ConfigurationSetting> Get(string key = KeyFilter.Any, string label = LabelFilter.Any, DateTime? utcPointInTime = default)
    {
        if (utcPointInTime.HasValue)
        {
            var outer = PredicateBuilder.New<ConfigurationSettingRevision>(revision => revision.ValidFrom <= utcPointInTime.Value && (revision.ValidTo == null || revision.ValidTo > utcPointInTime.Value));

            if (key != KeyFilter.Any)
            {
                var inner = PredicateBuilder.New<ConfigurationSettingRevision>(false);

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
                var inner = PredicateBuilder.New<ConfigurationSettingRevision>(false);

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

            return Context.ConfigurationSettingRevisions.Where(outer).Select(revision => new ConfigurationSetting(revision)).AsAsyncEnumerable();
        }
        else
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
    }

    public async Task RemoveAsync(ConfigurationSetting setting, CancellationToken cancellationToken = default)
    {
        var date = DateTime.UtcNow;

        Context.ConfigurationSettings.Remove(setting);

        var revision = await Context.ConfigurationSettingRevisions.SingleOrDefaultAsync(
            revision =>
                revision.Key == setting.Key &&
                revision.Label == setting.Label &&
                revision.ValidFrom <= date &&
                revision.ValidTo == null,
            cancellationToken);

        if (revision != null)
        {
            revision.ValidTo = date;

            Context.ConfigurationSettingRevisions.Update(revision);
        }

        await Context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(ConfigurationSetting setting, CancellationToken cancellationToken = default)
    {
        var date = DateTime.UtcNow;

        Context.ConfigurationSettings.Update(setting);

        var revision = await Context.ConfigurationSettingRevisions.SingleOrDefaultAsync(
            revision =>
                revision.Key == setting.Key &&
                revision.Label == setting.Label &&
                revision.ValidFrom <= date &&
                revision.ValidTo == null,
            cancellationToken);

        if (revision != null)
        {
            revision.ValidTo = setting.LastModified;

            Context.ConfigurationSettingRevisions.Update(revision);
        }

        Context.ConfigurationSettingRevisions.Add(new ConfigurationSettingRevision(setting));

        await Context.SaveChangesAsync(cancellationToken);
    }
}
