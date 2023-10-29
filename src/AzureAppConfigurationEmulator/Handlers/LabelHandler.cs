using System.Text.RegularExpressions;
using AzureAppConfigurationEmulator.Constants;
using AzureAppConfigurationEmulator.Contexts;
using AzureAppConfigurationEmulator.Extensions;
using AzureAppConfigurationEmulator.Results;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AzureAppConfigurationEmulator.Handlers;

public class LabelHandler
{
    public static async Task<Results<LabelSetResult, InvalidCharacterResult, TooManyValuesResult>> List(
        [FromServices] ApplicationDbContext context,
        CancellationToken cancellationToken,
        [FromQuery] string name = LabelFilter.Any)
    {
        if (name != LabelFilter.Any)
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

        var labels = await context.ConfigurationSettings
            .Where(label: name)
            .Select(setting => setting.Label.NormalizeNull())
            .Distinct()
            .ToListAsync(cancellationToken);

        return new LabelSetResult(labels);
    }
}
