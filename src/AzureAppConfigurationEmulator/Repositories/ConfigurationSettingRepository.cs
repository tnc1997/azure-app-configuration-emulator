using System.Data.Common;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.RegularExpressions;
using AzureAppConfigurationEmulator.Constants;
using AzureAppConfigurationEmulator.Entities;
using AzureAppConfigurationEmulator.Extensions;
using AzureAppConfigurationEmulator.Factories;

namespace AzureAppConfigurationEmulator.Repositories;

public partial class ConfigurationSettingRepository(
    IDbCommandFactory commandFactory,
    IConfigurationSettingFactory configurationSettingFactory,
    IDbConnectionFactory connectionFactory,
    ILogger<ConfigurationSettingRepository> logger,
    IDbParameterFactory parameterFactory) : IConfigurationSettingRepository
{
    private IDbCommandFactory CommandFactory { get; } = commandFactory;

    private IConfigurationSettingFactory ConfigurationSettingFactory { get; } = configurationSettingFactory;
    
    private IDbConnectionFactory ConnectionFactory { get; } = connectionFactory;

    private ILogger<ConfigurationSettingRepository> Logger { get; } = logger;
    
    private IDbParameterFactory ParameterFactory { get; } = parameterFactory;

    public async Task Add(
        ConfigurationSetting setting,
        CancellationToken cancellationToken = default)
    {
        using var activity = Telemetry.ActivitySource.StartActivity($"{nameof(ConfigurationSettingRepository)}.{nameof(Add)}");
        activity?.SetTag(Telemetry.ConfigurationSettingEtag, setting.Etag);
        activity?.SetTag(Telemetry.ConfigurationSettingKey, setting.Key);
        activity?.SetTag(Telemetry.ConfigurationSettingLabel, setting.Label);
        activity?.SetTag(Telemetry.ConfigurationSettingContentType, setting.ContentType);
        activity?.SetTag(Telemetry.ConfigurationSettingValue, setting.Value);
        activity?.SetTag(Telemetry.ConfigurationSettingLastModified, setting.LastModified);
        activity?.SetTag(Telemetry.ConfigurationSettingLocked, setting.Locked);

        const string text = "INSERT INTO configuration_settings (etag, key, label, content_type, value, last_modified, locked, tags) VALUES ($etag, $key, $label, $content_type, $value, $last_modified, $locked, $tags)";

        var parameters = new List<DbParameter>
        {
            ParameterFactory.Create("$etag", setting.Etag),
            ParameterFactory.Create("$key", setting.Key),
            ParameterFactory.Create("$label", setting.Label),
            ParameterFactory.Create("$content_type", setting.ContentType),
            ParameterFactory.Create("$value", setting.Value),
            ParameterFactory.Create("$last_modified", setting.LastModified),
            ParameterFactory.Create("$locked", setting.Locked),
            ParameterFactory.Create("$tags", setting.Tags)
        };

        await ExecuteNonQuery(text, parameters, cancellationToken);
    }

    public async IAsyncEnumerable<ConfigurationSetting> Get(
        string key = KeyFilter.Any,
        string label = LabelFilter.Any,
        DateTimeOffset? moment = default,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        using var activity = Telemetry.ActivitySource.StartActivity($"{nameof(ConfigurationSettingRepository)}.{nameof(Get)}");

        var text = $"SELECT etag, key, label, content_type, value, last_modified, locked, tags FROM {(moment is not null ? "configuration_settings_history" : "configuration_settings")}";

        var parameters = new List<DbParameter>();

        var outers = new List<string>();

        if (key is not KeyFilter.Any)
        {
            var keys = UnescapedCommaRegex().Split(key).Select(s => s.Unescape()).ToList();

            var inners = new List<string>();

            for (var i = 0; i < keys.Count; i++)
            {
                var match = TrailingWildcardRegex().Match(keys[i]);

                parameters.Add(ParameterFactory.Create($"$key{i}", match.Success ? $"{match.Groups[1].Value}%" : keys[i]));

                inners.Add(match.Success ? $"key LIKE $key{i}" : $"key = $key{i}");
            }

            outers.Add($"({string.Join(" OR ", inners)})");
        }

        if (label is not LabelFilter.Any)
        {
            var labels = UnescapedCommaRegex().Split(label).Select(s => s.Unescape()).ToList();

            var inners = new List<string>();

            for (var i = 0; i < labels.Count; i++)
            {
                if (labels[i] == LabelFilter.Null)
                {
                    inners.Add("label IS NULL");
                }
                else
                {
                    var match = TrailingWildcardRegex().Match(labels[i]);

                    parameters.Add(ParameterFactory.Create($"$label{i}", match.Success ? $"{match.Groups[1].Value}%" : labels[i]));

                    inners.Add(match.Success ? $"label LIKE $label{i}" : $"label = $label{i}");
                }
            }

            outers.Add($"({string.Join(" OR ", inners)})");
        }

        if (moment is not null)
        {
            parameters.Add(ParameterFactory.Create("$moment", moment));

            outers.Add("(valid_from <= $moment AND valid_to > $moment)");
        }

        if (outers.Count > 0)
        {
            text += $" WHERE {string.Join(" AND ", outers)}";
        }

        await foreach (var reader in ExecuteReader(text, parameters, cancellationToken))
        {
            yield return ConfigurationSettingFactory.Create(
                reader.GetString(0),
                reader.GetString(1),
                DateTimeOffset.Parse(reader.GetString(5), styles: DateTimeStyles.AssumeUniversal),
                reader.GetBoolean(6),
                reader.IsDBNull(2) ? null : reader.GetString(2),
                reader.IsDBNull(3) ? null : reader.GetString(3),
                reader.IsDBNull(4) ? null : reader.GetString(4),
                reader.IsDBNull(7) ? null : JsonSerializer.Deserialize<IReadOnlyDictionary<string, object?>>(reader.GetString(7)));
        }
    }

    public async Task Remove(
        ConfigurationSetting setting,
        CancellationToken cancellationToken = default)
    {
        using var activity = Telemetry.ActivitySource.StartActivity($"{nameof(ConfigurationSettingRepository)}.{nameof(Remove)}");
        activity?.SetTag(Telemetry.ConfigurationSettingEtag, setting.Etag);
        activity?.SetTag(Telemetry.ConfigurationSettingKey, setting.Key);
        activity?.SetTag(Telemetry.ConfigurationSettingLabel, setting.Label);
        activity?.SetTag(Telemetry.ConfigurationSettingContentType, setting.ContentType);
        activity?.SetTag(Telemetry.ConfigurationSettingValue, setting.Value);
        activity?.SetTag(Telemetry.ConfigurationSettingLastModified, setting.LastModified);
        activity?.SetTag(Telemetry.ConfigurationSettingLocked, setting.Locked);

        var text = "DELETE FROM configuration_settings";

        var parameters = new List<DbParameter> { ParameterFactory.Create("$key", setting.Key) };

        var outers = new List<string> { "key = $key" };

        if (setting.Label is not null)
        {
            parameters.Add(ParameterFactory.Create("$label", setting.Label));

            outers.Add("label = $label");
        }
        else
        {
            outers.Add("label IS NULL");
        }

        if (outers.Count > 0)
        {
            text += $" WHERE {string.Join(" AND ", outers)}";
        }

        await ExecuteNonQuery(text, parameters, cancellationToken);
    }

    public async Task Update(
        ConfigurationSetting setting,
        CancellationToken cancellationToken = default)
    {
        using var activity = Telemetry.ActivitySource.StartActivity($"{nameof(ConfigurationSettingRepository)}.{nameof(Update)}");
        activity?.SetTag(Telemetry.ConfigurationSettingEtag, setting.Etag);
        activity?.SetTag(Telemetry.ConfigurationSettingKey, setting.Key);
        activity?.SetTag(Telemetry.ConfigurationSettingLabel, setting.Label);
        activity?.SetTag(Telemetry.ConfigurationSettingContentType, setting.ContentType);
        activity?.SetTag(Telemetry.ConfigurationSettingValue, setting.Value);
        activity?.SetTag(Telemetry.ConfigurationSettingLastModified, setting.LastModified);
        activity?.SetTag(Telemetry.ConfigurationSettingLocked, setting.Locked);

        var text = "UPDATE configuration_settings SET etag = $etag, content_type = $content_type, value = $value, last_modified = $last_modified, locked = $locked, tags = $tags";

        var parameters = new List<DbParameter>
        {
            ParameterFactory.Create("$etag", setting.Etag),
            ParameterFactory.Create("$key", setting.Key),
            ParameterFactory.Create("$content_type", setting.ContentType),
            ParameterFactory.Create("$value", setting.Value),
            ParameterFactory.Create("$last_modified", setting.LastModified),
            ParameterFactory.Create("$locked", setting.Locked),
            ParameterFactory.Create("$tags", setting.Tags)
        };

        var outers = new List<string> { "key = $key" };

        if (setting.Label is not null)
        {
            parameters.Add(ParameterFactory.Create("$label", setting.Label));

            outers.Add("label = $label");
        }
        else
        {
            outers.Add("label IS NULL");
        }

        if (outers.Count > 0)
        {
            text += $" WHERE {string.Join(" AND ", outers)}";
        }

        await ExecuteNonQuery(text, parameters, cancellationToken);
    }

    [GeneratedRegex(@"^(.*)(?<!\\)\*$")]
    private static partial Regex TrailingWildcardRegex();

    [GeneratedRegex(@"(?<!\\),")]
    private static partial Regex UnescapedCommaRegex();

    private async Task ExecuteNonQuery(
        string text,
        IEnumerable<DbParameter> parameters,
        CancellationToken cancellationToken = default)
    {
        using var activity = Telemetry.ActivitySource.StartActivity($"{nameof(ConfigurationSettingRepository)}.{nameof(ExecuteNonQuery)}");
        activity?.SetTag(Telemetry.DatabaseStatement, text);

        Logger.LogDebug("Creating a connection.");
        await using var connection = ConnectionFactory.Create();

        Logger.LogDebug("Opening the connection.");
        await connection.OpenAsync(cancellationToken);

        Logger.LogDebug("Creating a command.");
        await using var command = CommandFactory.Create(connection, text, parameters);

        Logger.LogDebug("Executing the command.");
        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    private async IAsyncEnumerable<DbDataReader> ExecuteReader(
        string text,
        IEnumerable<DbParameter> parameters,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        using var activity = Telemetry.ActivitySource.StartActivity($"{nameof(ConfigurationSettingRepository)}.{nameof(ExecuteReader)}");
        activity?.SetTag(Telemetry.DatabaseStatement, text);

        Logger.LogDebug("Creating a connection.");
        await using var connection = ConnectionFactory.Create();

        Logger.LogDebug("Opening the connection.");
        await connection.OpenAsync(cancellationToken);

        Logger.LogDebug("Creating a command.");
        await using var command = CommandFactory.Create(connection, text, parameters);

        Logger.LogDebug("Executing the command.");
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        Logger.LogDebug("Reading the results.");
        while (await reader.ReadAsync(cancellationToken))
        {
            yield return reader;
        }
    }
}
