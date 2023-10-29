using System.Text.RegularExpressions;
using AzureAppConfigurationEmulator.Constants;
using AzureAppConfigurationEmulator.Contexts;
using AzureAppConfigurationEmulator.Extensions;
using AzureAppConfigurationEmulator.Results;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AzureAppConfigurationEmulator.Handlers;

public class KeyHandler
{
    public static async Task<Results<KeySetResult, InvalidCharacterResult, TooManyValuesResult>> List(
        [FromServices] ApplicationDbContext context,
        CancellationToken cancellationToken,
        [FromQuery] string name = KeyFilter.Any)
    {
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

        var keys = await context.ConfigurationSettings
            .Where(key: name)
            .Select(setting => setting.Key)
            .ToListAsync(cancellationToken);

        return new KeySetResult(keys);
    }
}
