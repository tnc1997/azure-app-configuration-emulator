using System.Data.Common;

namespace AzureAppConfigurationEmulator.Data;

public class SqliteDbCommandFactory(ILogger<SqliteDbCommandFactory>? logger = null) : IDbCommandFactory
{
    public DbCommand Create(DbConnection connection, string? text = null, IEnumerable<DbParameter>? parameters = null)
    {
        logger?.LogDebug("Creating a command.");
        var command = connection.CreateCommand();

        if (text is not null)
        {
            logger?.LogDebug("Setting the command text.");
            command.CommandText = text;
        }

        if (parameters is not null)
        {
            logger?.LogDebug("Enumerating the parameters.");
            foreach (var parameter in parameters)
            {
                logger?.LogDebug("Adding the parameter with the name '{ParameterName}' and the value '{ParameterValue}'.", parameter.ParameterName, parameter.Value);
                command.Parameters.Add(parameter);
            }
        }

        return command;
    }
}
