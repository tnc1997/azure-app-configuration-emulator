using System.Text.RegularExpressions;
using AzureAppConfigurationEmulator.Constants;
using AzureAppConfigurationEmulator.Entities;
using AzureAppConfigurationEmulator.Repositories;
using AzureAppConfigurationEmulator.Results;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace AzureAppConfigurationEmulator.Handlers;

public class KeyValueHandler
{
    public static async Task<Results<KeyValueResult, NoContent>> Delete(
        [FromServices] IConfigurationSettingRepository repository,
        [FromRoute] string key,
        [FromQuery] string label = LabelFilter.Null,
        CancellationToken cancellationToken = default)
    {
        var setting = await repository.Get(key, label).SingleOrDefaultAsync(cancellationToken);

        if (setting == null)
        {
            return TypedResults.NoContent();
        }

        await repository.RemoveAsync(setting, cancellationToken);

        return new KeyValueResult(setting);
    }

    public static async Task<Results<KeyValueResult, NotFound>> Get(
        [FromServices] IConfigurationSettingRepository repository,
        [FromRoute] string key,
        [FromQuery] string label = LabelFilter.Null,
        CancellationToken cancellationToken = default)
    {
        var setting = await repository.Get(key, label).SingleOrDefaultAsync(cancellationToken);

        return setting != null ? new KeyValueResult(setting) : TypedResults.NotFound();
    }

    public static async Task<Results<KeyValueSetResult, InvalidCharacterResult, TooManyValuesResult>> List(
        [FromServices] IConfigurationSettingRepository repository,
        [FromQuery] string key = KeyFilter.Any,
        [FromQuery] string label = LabelFilter.Any,
        CancellationToken cancellationToken = default)
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

        var settings = await repository.Get(key, label).ToListAsync(cancellationToken);

        return new KeyValueSetResult(settings);
    }

    public static async Task<Results<KeyValueResult, ReadOnlyResult>> Set(
        [FromServices] IConfigurationSettingRepository repository,
        [FromBody] SetInput input,
        [FromRoute] string key,
        [FromQuery] string label = LabelFilter.Null,
        CancellationToken cancellationToken = default)
    {
        var setting = await repository.Get(key, label).SingleOrDefaultAsync(cancellationToken);

        if (setting == null)
        {
            setting = new ConfigurationSetting(
                key,
                label,
                input.ContentType,
                input.Value,
                DateTimeOffset.UtcNow,
                false);

            await repository.AddAsync(setting, cancellationToken);

            return new KeyValueResult(setting);
        }

        if (setting.IsReadOnly)
        {
            return new ReadOnlyResult(key);
        }

        setting.Value = input.Value;
        setting.ContentType = input.ContentType;

        await repository.UpdateAsync(setting, cancellationToken);

        return new KeyValueResult(setting);
    }

    public record SetInput(string? Value, string? ContentType);
}
