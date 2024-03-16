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
        using var activity = Telemetry.ActivitySource.StartActivity($"{nameof(KeyValueHandler)}.{nameof(Delete)}");
        activity?.SetTag(Telemetry.QueryLabel, label);

        ifMatch = ifMatch?.TrimStart('"').TrimEnd('"');
        activity?.SetTag(Telemetry.HeaderIfMatch, ifMatch);
        ifNoneMatch = ifNoneMatch?.TrimStart('"').TrimEnd('"');
        activity?.SetTag(Telemetry.HeaderIfNoneMatch, ifNoneMatch);
        key = Uri.UnescapeDataString(key);
        activity?.SetTag(Telemetry.RouteKey, key);

        var setting = await repository.Get(key, label, cancellationToken: cancellationToken).SingleOrDefaultAsync(cancellationToken);

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

        await repository.Remove(setting, cancellationToken);

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
        using var activity = Telemetry.ActivitySource.StartActivity($"{nameof(KeyValueHandler)}.{nameof(Get)}");
        activity?.SetTag(Telemetry.QueryLabel, label);
        activity?.SetTag(Telemetry.HeaderAcceptDatetime, acceptDatetime);

        ifMatch = ifMatch?.TrimStart('"').TrimEnd('"');
        activity?.SetTag(Telemetry.HeaderIfMatch, ifMatch);
        ifNoneMatch = ifNoneMatch?.TrimStart('"').TrimEnd('"');
        activity?.SetTag(Telemetry.HeaderIfNoneMatch, ifNoneMatch);
        key = Uri.UnescapeDataString(key);
        activity?.SetTag(Telemetry.RouteKey, key);

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
        using var activity = Telemetry.ActivitySource.StartActivity($"{nameof(KeyValueHandler)}.{nameof(List)}");
        activity?.SetTag(Telemetry.QueryKey, key);
        activity?.SetTag(Telemetry.QueryLabel, label);
        activity?.SetTag(Telemetry.HeaderAcceptDatetime, acceptDatetime);

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
        using var activity = Telemetry.ActivitySource.StartActivity($"{nameof(KeyValueHandler)}.{nameof(Set)}");
        activity?.SetTag(Telemetry.QueryLabel, label);

        ifMatch = ifMatch?.TrimStart('"').TrimEnd('"');
        activity?.SetTag(Telemetry.HeaderIfMatch, ifMatch);
        ifNoneMatch = ifNoneMatch?.TrimStart('"').TrimEnd('"');
        activity?.SetTag(Telemetry.HeaderIfNoneMatch, ifNoneMatch);
        key = Uri.UnescapeDataString(key);
        activity?.SetTag(Telemetry.RouteKey, key);

        var date = DateTimeOffset.UtcNow;

        var setting = await repository.Get(key, label, cancellationToken: cancellationToken).SingleOrDefaultAsync(cancellationToken);

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
                date,
                false,
                label is LabelFilter.Null ? null : label,
                input.ContentType,
                input.Value,
                input.Tags);

            await repository.Add(setting, cancellationToken);

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

        setting = setting with
        {
            Etag = Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(date.UtcDateTime.ToString("yyyy-MM-dd HH:mm:ss")))),
            ContentType = input.ContentType,
            Value = input.Value,
            LastModified = date,
            Tags = input.Tags
        };

        await repository.Update(setting, cancellationToken);

        return new KeyValueResult(setting);
    }

    public record SetInput(string? Value, string? ContentType, IReadOnlyDictionary<string, object?>? Tags);
}
