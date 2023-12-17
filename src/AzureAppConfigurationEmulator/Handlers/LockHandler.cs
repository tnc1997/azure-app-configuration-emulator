using AzureAppConfigurationEmulator.Constants;
using AzureAppConfigurationEmulator.Repositories;
using AzureAppConfigurationEmulator.Results;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace AzureAppConfigurationEmulator.Handlers;

public class LockHandler
{
    public static async Task<Results<KeyValueResult, NotFound>> Lock(
        [FromServices] IConfigurationSettingRepository repository,
        [FromRoute] string key,
        [FromQuery] string label = LabelFilter.Null,
        CancellationToken cancellationToken = default)
    {
        var setting = await repository.Get(key, label).SingleOrDefaultAsync(cancellationToken);

        if (setting == null)
        {
            return TypedResults.NotFound();
        }

        setting.IsReadOnly = true;

        await repository.UpdateAsync(setting, cancellationToken);

        return new KeyValueResult(setting);
    }

    public static async Task<Results<KeyValueResult, NotFound>> Unlock(
        [FromServices] IConfigurationSettingRepository repository,
        [FromRoute] string key,
        [FromQuery] string label = LabelFilter.Null,
        CancellationToken cancellationToken = default)
    {
        var setting = await repository.Get(key, label).SingleOrDefaultAsync(cancellationToken);

        if (setting == null)
        {
            return TypedResults.NotFound();
        }

        setting.IsReadOnly = false;

        await repository.UpdateAsync(setting, cancellationToken);

        return new KeyValueResult(setting);
    }
}
