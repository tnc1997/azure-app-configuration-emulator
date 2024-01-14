using System.Collections;
using System.Data.Common;

namespace AzureAppConfigurationEmulator.Factories;

public class DbCommandFactory(ILogger<DbCommandFactory>? logger = null) : IDbCommandFactory
{
    private ILogger<DbCommandFactory>? Logger { get; } = logger;

    public DbCommand Create(DbConnection connection, string? text = null, IEnumerable<DbParameter>? parameters = null)
    {
        Logger?.LogDebug("Creating the command.");
        var command = connection.CreateCommand();

        if (text is not null)
        {
            Logger?.LogDebug("Setting the command text.");
            command.CommandText = text;
        }

        if (parameters is not null)
        {
            Logger?.LogDebug("Enumerating the parameters.");
            foreach (var parameter in parameters)
            {
                using (Logger?.BeginScope(new DbParameterLogScope(parameter)))
                {
                    Logger?.LogDebug("Adding the parameter.");
                    command.Parameters.Add(parameter);
                }
            }
        }

        return command;
    }

    private class DbParameterLogScope(DbParameter parameter) : IEnumerable<KeyValuePair<string, object?>>
    {
        public IEnumerator<KeyValuePair<string, object?>> GetEnumerator()
        {
            yield return new KeyValuePair<string, object?>("ParameterName", parameter.ParameterName);

            yield return new KeyValuePair<string, object?>("ParameterValue", parameter.Value);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public override string ToString()
        {
            return $"ParameterName:{parameter.ParameterName} ParameterValue:{parameter.Value}";
        }
    }
}
