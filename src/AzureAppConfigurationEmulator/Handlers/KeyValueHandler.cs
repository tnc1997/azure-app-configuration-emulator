using System.Security.Cryptography;
using System.Text;
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
    public static async Task<Results<KeyValueResult, NoContent, PreconditionFailedResult, ReadOnlyResult>> Delete(
        [FromServices] IConfigurationSettingRepository repository,
        [FromRoute] string key,
        [FromQuery] string label = LabelFilter.Null,
        [FromHeader(Name = "If-Match")] string? ifMatch = default,
        [FromHeader(Name = "If-None-Match")] string? ifNoneMatch = default,
        CancellationToken cancellationToken = default)
    {
        ifMatch = ifMatch?.TrimStart('"').TrimEnd('"');
        ifNoneMatch = ifNoneMatch?.TrimStart('"').TrimEnd('"');
        key = Uri.UnescapeDataString(key);

        var setting = await repository.Get(key, label, cancellationToken).SingleOrDefaultAsync(cancellationToken);

        if (setting == null)
        {
            if (ifMatch != null && ifMatch == "*")
            {
                return new PreconditionFailedResult();
            }

            if (ifNoneMatch != null && ifNoneMatch != "*")
            {
                return new PreconditionFailedResult();
            }

            return TypedResults.NoContent();
        }

        if (setting.Locked)
        {
            return new ReadOnlyResult(key);
        }

        if (ifMatch != null && (ifMatch != setting.Etag && ifMatch != "*"))
        {
            return new PreconditionFailedResult();
        }

        if (ifNoneMatch != null && (ifNoneMatch == setting.Etag || ifNoneMatch == "*"))
        {
            return new PreconditionFailedResult();
        }

        await repository.RemoveAsync(setting, cancellationToken);

        return new KeyValueResult(setting);
    }

    public static async Task<Results<KeyValueResult, NotFound, NotModifiedResult, PreconditionFailedResult>> Get(
        [FromServices] IConfigurationSettingRepository repository,
        [FromRoute] string key,
        [FromQuery] string label = LabelFilter.Null,
        [FromHeader(Name = "Accept-Datetime")] DateTimeOffset? acceptDatetime = default,
        [FromHeader(Name = "If-Match")] string? ifMatch = default,
        [FromHeader(Name = "If-None-Match")] string? ifNoneMatch = default,
        CancellationToken cancellationToken = default)
    {
        ifMatch = ifMatch?.TrimStart('"').TrimEnd('"');
        ifNoneMatch = ifNoneMatch?.TrimStart('"').TrimEnd('"');
        key = Uri.UnescapeDataString(key);

        var setting = await repository.Get(key, label, acceptDatetime, cancellationToken).SingleOrDefaultAsync(cancellationToken);

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
            return new NotModifiedResult();
        }

        return new KeyValueResult(setting, acceptDatetime);
    }

    public static async Task<Results<KeyValueSetResult, InvalidCharacterResult, TooManyValuesResult>> List(
        [FromServices] IConfigurationSettingRepository repository,
        [FromQuery] string key = KeyFilter.Any,
        [FromQuery] string label = LabelFilter.Any,
        [FromHeader(Name = "Accept-Datetime")] DateTimeOffset? acceptDatetime = default,
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

        var settings = await repository.Get(key, label, acceptDatetime, cancellationToken).ToListAsync(cancellationToken);

        return new KeyValueSetResult(settings, acceptDatetime);
    }

    public static async Task<Results<KeyValueResult, PreconditionFailedResult, ReadOnlyResult>> Set(
        [FromServices] IConfigurationSettingRepository repository,
        [FromBody] SetInput input,
        [FromRoute] string key,
        [FromQuery] string label = LabelFilter.Null,
        [FromHeader(Name = "If-Match")] string? ifMatch = default,
        [FromHeader(Name = "If-None-Match")] string? ifNoneMatch = default,
        CancellationToken cancellationToken = default)
    {
        ifMatch = ifMatch?.TrimStart('"').TrimEnd('"');
        ifNoneMatch = ifNoneMatch?.TrimStart('"').TrimEnd('"');
        key = Uri.UnescapeDataString(key);

        var date = DateTime.UtcNow;

        var setting = await repository.Get(key, label, cancellationToken).SingleOrDefaultAsync(cancellationToken);

        if (setting == null)
        {
            if (ifMatch != null && ifMatch == "*")
            {
                return new PreconditionFailedResult();
            }

            if (ifNoneMatch != null && ifNoneMatch != "*")
            {
                return new PreconditionFailedResult();
            }

            setting = new ConfigurationSetting(
                Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(date.UtcDateTime.ToString("yyyy-MM-dd HH:mm:ss")))),
                key,
                label is LabelFilter.Null ? null : label,
                input.ContentType,
                input.Value,
                date,
                false,
                input.Tags);

            await repository.AddAsync(setting, cancellationToken);

            return new KeyValueResult(setting);
        }

        if (setting.Locked)
        {
            return new ReadOnlyResult(key);
        }

        if (ifMatch != null && (ifMatch != setting.Etag && ifMatch != "*"))
        {
            return new PreconditionFailedResult();
        }

        if (ifNoneMatch != null && (ifNoneMatch == setting.Etag || ifNoneMatch == "*"))
        {
            return new PreconditionFailedResult();
        }

        setting.Etag = Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(date.UtcDateTime.ToString("yyyy-MM-dd HH:mm:ss"))));
        setting.ContentType = input.ContentType;
        setting.Value = input.Value;
        setting.LastModified = date;
        setting.Tags = input.Tags;

        await repository.UpdateAsync(setting, cancellationToken);

        return new KeyValueResult(setting);
    }

    public record SetInput(string? Value, string? ContentType, IDictionary<string, object?>? Tags);
}
