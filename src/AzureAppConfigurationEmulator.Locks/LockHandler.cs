using AzureAppConfigurationEmulator.Common;
using AzureAppConfigurationEmulator.Common.Abstractions;
using AzureAppConfigurationEmulator.Common.Constants;
using AzureAppConfigurationEmulator.Common.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace AzureAppConfigurationEmulator.Locks;

public class LockHandler
{
    public static async Task<Results<ConfigurationSettingResult, NotFound, PreconditionFailedResult>> Lock(
        [FromServices] IConfigurationSettingRepository repository,
        [FromRoute] string key,
        [FromQuery] string label = LabelFilter.Null,
        [FromHeader(Name = "If-Match")] string? ifMatch = default,
        [FromHeader(Name = "If-None-Match")] string? ifNoneMatch = default,
        CancellationToken cancellationToken = default)
    {
        using var activity = Telemetry.ActivitySource.StartActivity($"{nameof(LockHandler)}.{nameof(Lock)}");
        activity?.SetTag(Telemetry.RouteKey, key);
        activity?.SetTag(Telemetry.QueryLabel, label);

        ifMatch = ifMatch?.TrimStart('"').TrimEnd('"');
        activity?.SetTag(Telemetry.HeaderIfMatch, ifMatch);
        ifNoneMatch = ifNoneMatch?.TrimStart('"').TrimEnd('"');
        activity?.SetTag(Telemetry.HeaderIfNoneMatch, ifNoneMatch);

        var setting = await repository.Get(key, label).SingleOrDefaultAsync(cancellationToken);

        if (setting == null)
        {
            return TypedResults.NotFound();
        }

        if (ifMatch != null && (ifMatch != setting.Etag && ifMatch != "*"))
        {
            return new PreconditionFailedResult();
        }

        if (ifNoneMatch != null && (ifNoneMatch == setting.Etag || ifNoneMatch == "*"))
        {
            return new PreconditionFailedResult();
        }

        setting = setting with { Locked = true };

        await repository.Update(setting, cancellationToken);

        return new ConfigurationSettingResult(setting);

    }

    public static async Task<Results<ConfigurationSettingResult, NotFound, PreconditionFailedResult>> Unlock(
        [FromServices] IConfigurationSettingRepository repository,
        [FromRoute] string key,
        [FromQuery] string label = LabelFilter.Null,
        [FromHeader(Name = "If-Match")] string? ifMatch = default,
        [FromHeader(Name = "If-None-Match")] string? ifNoneMatch = default,
        CancellationToken cancellationToken = default)
    {
        using var activity = Telemetry.ActivitySource.StartActivity($"{nameof(LockHandler)}.{nameof(Unlock)}");
        activity?.SetTag(Telemetry.RouteKey, key);
        activity?.SetTag(Telemetry.QueryLabel, label);

        ifMatch = ifMatch?.TrimStart('"').TrimEnd('"');
        activity?.SetTag(Telemetry.HeaderIfMatch, ifMatch);
        ifNoneMatch = ifNoneMatch?.TrimStart('"').TrimEnd('"');
        activity?.SetTag(Telemetry.HeaderIfNoneMatch, ifNoneMatch);

        var setting = await repository.Get(key, label).SingleOrDefaultAsync(cancellationToken);

        if (setting == null)
        {
            return TypedResults.NotFound();
        }

        if (ifMatch != null && (ifMatch != setting.Etag && ifMatch != "*"))
        {
            return new PreconditionFailedResult();
        }

        if (ifNoneMatch != null && (ifNoneMatch == setting.Etag || ifNoneMatch == "*"))
        {
            return new PreconditionFailedResult();
        }

        setting = setting with { Locked = false };

        await repository.Update(setting, cancellationToken);

        return new ConfigurationSettingResult(setting);
    }
}
