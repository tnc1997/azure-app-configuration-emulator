using AzureAppConfigurationEmulator.Constants;
using AzureAppConfigurationEmulator.Repositories;
using AzureAppConfigurationEmulator.Results;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace AzureAppConfigurationEmulator.Handlers;

public class LockHandler
{
    public static async Task<Results<KeyValueResult, NotFound, PreconditionFailedResult>> Lock(
        [FromServices] IConfigurationSettingRepository repository,
        [FromRoute] string key,
        [FromQuery] string label = LabelFilter.Null,
        [FromHeader(Name = "If-Match")] string? ifMatch = default,
        [FromHeader(Name = "If-None-Match")] string? ifNoneMatch = default,
        CancellationToken cancellationToken = default)
    {
        ifMatch = ifMatch?.TrimStart('"').TrimEnd('"');
        ifNoneMatch = ifNoneMatch?.TrimStart('"').TrimEnd('"');

        var setting = await repository.Get(key, label).SingleOrDefaultAsync(cancellationToken);

        if (setting == null)
        {
            return TypedResults.NotFound();
        }

        if (ifMatch != null && (ifMatch != setting.ETag && ifMatch != "*"))
        {
            return new PreconditionFailedResult();
        }

        if (ifNoneMatch != null && (ifNoneMatch == setting.ETag || ifNoneMatch == "*"))
        {
            return new PreconditionFailedResult();
        }

        setting.IsReadOnly = true;

        await repository.UpdateAsync(setting, cancellationToken);

        return new KeyValueResult(setting);

    }

    public static async Task<Results<KeyValueResult, NotFound, PreconditionFailedResult>> Unlock(
        [FromServices] IConfigurationSettingRepository repository,
        [FromRoute] string key,
        [FromQuery] string label = LabelFilter.Null,
        [FromHeader(Name = "If-Match")] string? ifMatch = default,
        [FromHeader(Name = "If-None-Match")] string? ifNoneMatch = default,
        CancellationToken cancellationToken = default)
    {
        ifMatch = ifMatch?.TrimStart('"').TrimEnd('"');
        ifNoneMatch = ifNoneMatch?.TrimStart('"').TrimEnd('"');

        var setting = await repository.Get(key, label).SingleOrDefaultAsync(cancellationToken);

        if (setting == null)
        {
            return TypedResults.NotFound();
        }

        if (ifMatch != null && (ifMatch != setting.ETag && ifMatch != "*"))
        {
            return new PreconditionFailedResult();
        }

        if (ifNoneMatch != null && (ifNoneMatch == setting.ETag || ifNoneMatch == "*"))
        {
            return new PreconditionFailedResult();
        }

        setting.IsReadOnly = false;

        await repository.UpdateAsync(setting, cancellationToken);

        return new KeyValueResult(setting);
    }
}
