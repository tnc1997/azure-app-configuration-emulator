using System.Text.RegularExpressions;
using AzureAppConfigurationEmulator.Constants;
using AzureAppConfigurationEmulator.Contexts;
using AzureAppConfigurationEmulator.Entities;
using AzureAppConfigurationEmulator.Extensions;
using AzureAppConfigurationEmulator.Results;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AzureAppConfigurationEmulator.Handlers;

public class KeyValueHandler
{
    public static async Task<Results<KeyValueResult, NotFound>> Get(
        [FromServices] ApplicationDbContext context,
        [FromRoute] string key,
        CancellationToken cancellationToken,
        [FromQuery] string label = LabelFilter.Null)
    {
        var setting = await context.ConfigurationSettings.SingleOrDefaultAsync(
            setting => setting.Key == key && setting.Label == label,
            cancellationToken);

        return setting != null ? new KeyValueResult(setting) : TypedResults.NotFound();
    }

    public static async Task<Results<KeyValueSetResult, InvalidCharacterResult, TooManyValuesResult>> List(
        [FromServices] ApplicationDbContext context,
        CancellationToken cancellationToken,
        [FromQuery] string key = KeyFilter.Any,
        [FromQuery] string label = LabelFilter.Any)
    {
        if (key != KeyFilter.Any)
        {
            if (new Regex(@"(?=.*(?<!\\),)(?=.*\*)").IsMatch(key))
            {
                return new InvalidCharacterResult(nameof(key));
            }

            if (new Regex(@"(?:.*(?<!\\),){5,}").IsMatch(key))
            {
                return new TooManyValuesResult(nameof(key));
            }
        }

        if (label != LabelFilter.Any)
        {
            if (new Regex(@"(?=.*(?<!\\),)(?=.*\*)").IsMatch(label))
            {
                return new InvalidCharacterResult(nameof(label));
            }

            if (new Regex(@"(?:.*(?<!\\),){5,}").IsMatch(label))
            {
                return new TooManyValuesResult(nameof(label));
            }
        }

        var settings = await context.ConfigurationSettings
            .Where(key, label)
            .ToListAsync(cancellationToken);

        return new KeyValueSetResult(settings);
    }

    public static async Task<KeyValueResult> Set(
        [FromServices] ApplicationDbContext context,
        [FromBody] SetInput input,
        [FromRoute] string key,
        CancellationToken cancellationToken,
        [FromQuery] string label = LabelFilter.Null)
    {
        var setting = await context.ConfigurationSettings.SingleOrDefaultAsync(
            setting => setting.Key == key && setting.Label == label,
            cancellationToken);

        if (setting == null)
        {
            setting = new ConfigurationSetting(
                key,
                label,
                input.ContentType,
                input.Value,
                DateTimeOffset.UtcNow,
                false);

            context.ConfigurationSettings.Add(setting);
        }
        else
        {
            setting.Value = input.Value;
            setting.ContentType = input.ContentType;

            context.ConfigurationSettings.Update(setting);
        }

        await context.SaveChangesAsync(cancellationToken);

        return new KeyValueResult(setting);
    }

    public record SetInput(string? Value, string? ContentType);
}
