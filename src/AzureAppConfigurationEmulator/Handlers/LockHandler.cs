using AzureAppConfigurationEmulator.Constants;
using AzureAppConfigurationEmulator.Contexts;
using AzureAppConfigurationEmulator.Results;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AzureAppConfigurationEmulator.Handlers;

public class LockHandler
{
    public static async Task<Results<KeyValueResult, NotFound>> Lock(
        [FromServices] ApplicationDbContext context,
        [FromRoute] string key,
        CancellationToken cancellationToken,
        [FromQuery] string label = LabelFilter.Null)
    {
        var setting = await context.ConfigurationSettings.SingleOrDefaultAsync(
            setting => setting.Key == key && setting.Label == label,
            cancellationToken);

        if (setting == null)
        {
            return TypedResults.NotFound();
        }

        setting.IsReadOnly = true;

        context.ConfigurationSettings.Update(setting);

        await context.SaveChangesAsync(cancellationToken);

        return new KeyValueResult(setting);
    }
}
