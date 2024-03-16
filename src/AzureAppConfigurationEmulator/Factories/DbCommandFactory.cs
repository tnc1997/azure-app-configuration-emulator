using System.Data.Common;

namespace AzureAppConfigurationEmulator.Factories;

public class DbCommandFactory(ILogger<DbCommandFactory>? logger = null) : IDbCommandFactory
{
    private ILogger<DbCommandFactory>? Logger { get; } = logger;

    public DbCommand Create(DbConnection connection, string? text = null, IEnumerable<DbParameter>? parameters = null)
    {
        Logger?.LogDebug("Creating a command.");
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
                Logger?.LogDebug("Adding the parameter with the name '{ParameterName}' and the value '{ParameterValue}'.", parameter.ParameterName, parameter.Value);
                command.Parameters.Add(parameter);
            }
        }

        return command;
    }
}
