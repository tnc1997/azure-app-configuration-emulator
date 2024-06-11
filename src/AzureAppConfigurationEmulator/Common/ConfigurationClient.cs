using System.Runtime.CompilerServices;
using System.Text.Json;
using AzureAppConfigurationEmulator.ConfigurationSettings;

namespace AzureAppConfigurationEmulator.Common;

public class ConfigurationClient(
    HttpClient httpClient,
    IConfigurationSettingFactory configurationSettingFactory) : IConfigurationClient
{
    public async IAsyncEnumerable<ConfigurationSetting> GetConfigurationSettings(
        string key = KeyFilter.Any,
        string label = LabelFilter.Any,
        DateTimeOffset? moment = default,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        using var activity = Telemetry.ActivitySource.StartActivity($"{nameof(ConfigurationClient)}.{nameof(GetConfigurationSettings)}");

        LinkHeaderValue? link = null;

        do
        {
            var uri = link?.Next?.SingleOrDefault() ??
                      new Uri($"/kv?key={key}&label={label}&api-version=1.0", UriKind.Relative);

            using var request = new HttpRequestMessage(HttpMethod.Get, uri);

            if (moment.HasValue)
            {
                request.Headers.Add("Accept-Datetime", moment.Value.ToString("R"));
            }

            using var response = await httpClient.SendAsync(request, cancellationToken);

            response.EnsureSuccessStatusCode();

            link = response.Headers.TryGetValues("Link", out var values)
                ? LinkHeaderValue.Parse(string.Join(", ", values))
                : null;

            await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
            using var document = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken);

            foreach (var element in document.RootElement.GetProperty("items").EnumerateArray())
            {
                yield return configurationSettingFactory.Create(
                    element.GetProperty("etag").GetString()!,
                    element.GetProperty("key").GetString()!,
                    element.GetProperty("last_modified").GetDateTimeOffset(),
                    element.GetProperty("locked").GetBoolean(),
                    element.TryGetProperty("label", out var labelElement)
                        ? labelElement.GetString()
                        : null,
                    element.TryGetProperty("content_type", out var contentTypeElement)
                        ? contentTypeElement.GetString()
                        : null,
                    element.TryGetProperty("value", out var valueElement)
                        ? valueElement.GetString()
                        : null,
                    element.TryGetProperty("tags", out var tagsElement)
                        ? tagsElement.EnumerateObject().ToDictionary(
                            property => property.Name,
                            property => property.Value.GetString()!)
                        : null);
            }
        } while (link is { Next: not null });
    }

    public async IAsyncEnumerable<string> GetKeys(
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        using var activity = Telemetry.ActivitySource.StartActivity($"{nameof(ConfigurationClient)}.{nameof(GetKeys)}");

        LinkHeaderValue? link = null;

        do
        {
            var uri = link?.Next?.SingleOrDefault() ??
                      new Uri("/keys?api-version=1.0", UriKind.Relative);

            using var request = new HttpRequestMessage(HttpMethod.Get, uri);
            using var response = await httpClient.SendAsync(request, cancellationToken);

            response.EnsureSuccessStatusCode();

            link = response.Headers.TryGetValues("Link", out var values)
                ? LinkHeaderValue.Parse(string.Join(", ", values))
                : null;

            await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
            using var document = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken);

            foreach (var element in document.RootElement.GetProperty("items").EnumerateArray())
            {
                yield return element.GetProperty("name").GetString()!;
            }
        } while (link is { Next: not null });
    }

    public async IAsyncEnumerable<string?> GetLabels(
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        using var activity = Telemetry.ActivitySource.StartActivity($"{nameof(ConfigurationClient)}.{nameof(GetLabels)}");

        LinkHeaderValue? link = null;

        do
        {
            var uri = link?.Next?.SingleOrDefault() ??
                      new Uri("/labels?api-version=1.0", UriKind.Relative);

            using var request = new HttpRequestMessage(HttpMethod.Get, uri);
            using var response = await httpClient.SendAsync(request, cancellationToken);

            response.EnsureSuccessStatusCode();

            link = response.Headers.TryGetValues("Link", out var values)
                ? LinkHeaderValue.Parse(string.Join(", ", values))
                : null;

            await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
            using var document = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken);

            foreach (var element in document.RootElement.GetProperty("items").EnumerateArray())
            {
                yield return element.GetProperty("name").GetString();
            }
        } while (link is { Next: not null });
    }
}
