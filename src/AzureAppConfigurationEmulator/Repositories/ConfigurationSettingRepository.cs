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
    IDbConnectionFactory connectionFactory,
    ILogger<ConfigurationSettingRepository> logger,
    IDbParameterFactory parameterFactory) : IConfigurationSettingRepository
{
    private IDbCommandFactory CommandFactory { get; } = commandFactory;

    private IDbConnectionFactory ConnectionFactory { get; } = connectionFactory;

    private ILogger<ConfigurationSettingRepository> Logger { get; } = logger;
    
    private IDbParameterFactory ParameterFactory { get; } = parameterFactory;

    public async Task AddAsync(
        ConfigurationSetting setting,
        CancellationToken cancellationToken = default)
    {
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

        await ExecuteNonQueryAsync(text, parameters, cancellationToken);
    }

    public async IAsyncEnumerable<ConfigurationSetting> Get(
        string key = KeyFilter.Any,
        string label = LabelFilter.Any,
        DateTimeOffset? utcPointInTime = default,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var text = $"SELECT etag, key, label, content_type, value, last_modified, locked, tags FROM {(utcPointInTime is not null ? "configuration_settings_history" : "configuration_settings")}";

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

        if (utcPointInTime is not null)
        {
            parameters.Add(ParameterFactory.Create("$utc_point_in_time", utcPointInTime));

            outers.Add("(valid_from >= $utc_point_in_time AND valid_to < $utc_point_in_time)");
        }

        if (outers.Count > 0)
        {
            text += $" WHERE {string.Join(" AND ", outers)}";
        }

        await foreach (var reader in ExecuteReader(text, parameters, cancellationToken))
        {
            yield return new ConfigurationSetting(
                reader.GetString(0),
                reader.GetString(1),
                reader.IsDBNull(2) ? null : reader.GetString(2),
                reader.IsDBNull(3) ? null : reader.GetString(3),
                reader.IsDBNull(4) ? null : reader.GetString(4),
                DateTimeOffset.Parse(reader.GetString(5), styles: DateTimeStyles.AssumeUniversal),
                reader.GetBoolean(6),
                reader.IsDBNull(7) ? null : JsonSerializer.Deserialize<IDictionary<string, object?>>(reader.GetString(7)));
        }
    }

    public async Task RemoveAsync(
        ConfigurationSetting setting,
        CancellationToken cancellationToken = default)
    {
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

        await ExecuteNonQueryAsync(text, parameters, cancellationToken);
    }

    public async Task UpdateAsync(
        ConfigurationSetting setting,
        CancellationToken cancellationToken = default)
    {
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

        await ExecuteNonQueryAsync(text, parameters, cancellationToken);
    }

    [GeneratedRegex(@"^(.*)(?<!\\)\*$")]
    private static partial Regex TrailingWildcardRegex();

    [GeneratedRegex(@"(?<!\\),")]
    private static partial Regex UnescapedCommaRegex();

    private async Task ExecuteNonQueryAsync(
        string text,
        IEnumerable<DbParameter> parameters,
        CancellationToken cancellationToken = default)
    {
        using (Logger.BeginScope(new Dictionary<string, object?> { { "CommandText", text } }))
        {
            Logger.LogDebug("Creating the connection.");
            await using var connection = ConnectionFactory.Create();
            await connection.OpenAsync(cancellationToken);

            Logger.LogDebug("Creating the command.");
            await using var command = CommandFactory.Create(connection, text, parameters);

            Logger.LogDebug("Executing the command.");
            await command.ExecuteNonQueryAsync(cancellationToken);
        }
    }

    private async IAsyncEnumerable<DbDataReader> ExecuteReader(
        string text,
        IEnumerable<DbParameter> parameters,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        using (Logger.BeginScope(new Dictionary<string, object?> { { "CommandText", text } }))
        {
            Logger.LogDebug("Creating the connection.");
            await using var connection = ConnectionFactory.Create();
            await connection.OpenAsync(cancellationToken);

            Logger.LogDebug("Creating the command.");
            await using var command = CommandFactory.Create(connection, text, parameters);

            Logger.LogDebug("Executing the command.");
            await using var reader = await command.ExecuteReaderAsync(cancellationToken);
            while (await reader.ReadAsync(cancellationToken))
            {
                yield return reader;
            }
        }
    }
}
