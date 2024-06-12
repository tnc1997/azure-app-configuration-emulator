using System.Text.RegularExpressions;
using AzureAppConfigurationEmulator.Common;
using AzureAppConfigurationEmulator.ConfigurationSettings;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace AzureAppConfigurationEmulator.Keys;

public class KeyHandler
{
    public static async Task<Results<KeysResult, InvalidCharacterResult, TooManyValuesResult>> List(
        [FromServices] IConfigurationSettingRepository repository,
        [FromQuery] string name = KeyFilter.Any,
        [FromQuery(Name = "$select")] string? select = default,
        [FromHeader(Name = "Accept-Datetime")] DateTimeOffset? acceptDatetime = default,
        CancellationToken cancellationToken = default)
    {
        using var activity = Telemetry.ActivitySource.StartActivity($"{nameof(KeyHandler)}.{nameof(List)}");
        activity?.SetTag(Telemetry.QueryName, name);
        activity?.SetTag(Telemetry.QuerySelect, select);
        activity?.SetTag(Telemetry.HeaderAcceptDatetime, acceptDatetime);

        if (name != KeyFilter.Any)
        {
            if (new Regex(@"(?=.*(?<!\\),)(?=.*\*)").IsMatch(name))
            {
                return new InvalidCharacterResult(nameof(name));
            }

            if (new Regex(@"(?:.*(?<!\\),){5,}").IsMatch(name))
            {
                return new TooManyValuesResult(nameof(name));
            }
        }

        var keys = await repository.Get(key: name, moment: acceptDatetime, cancellationToken: cancellationToken)
            .Select(setting => setting.Key)
            .Distinct()
            .ToListAsync(cancellationToken);

        return new KeysResult(keys, acceptDatetime, select);
    }
}
