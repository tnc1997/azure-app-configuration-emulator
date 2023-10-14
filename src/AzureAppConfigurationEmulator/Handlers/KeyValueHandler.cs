using AzureAppConfigurationEmulator.Constants;
using AzureAppConfigurationEmulator.Contexts;
using AzureAppConfigurationEmulator.Entities;
using AzureAppConfigurationEmulator.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AzureAppConfigurationEmulator.Handlers;

public class KeyValueHandler
{
    public static async Task<KeyValueResult> Set(
        [FromServices] ApplicationDbContext context,
        [FromBody] SetInput input,
        [FromRoute] string key,
        CancellationToken cancellationToken,
        [FromQuery] string label = LabelFilter.Null)
    {
        var setting = await context.ConfigurationSettings.SingleOrDefaultAsync(
            setting => setting.Key == key && setting.Label == label,
            cancellationToken);

        if (setting == null)
        {
            setting = new ConfigurationSetting(
                key,
                label,
                input.ContentType,
                input.Value,
                DateTimeOffset.UtcNow,
                false);

            context.ConfigurationSettings.Add(setting);
        }
        else
        {
            setting.Value = input.Value;
            setting.ContentType = input.ContentType;

            context.ConfigurationSettings.Update(setting);
        }

        await context.SaveChangesAsync(cancellationToken);

        return new KeyValueResult(setting);
    }

    public record SetInput(string? Value, string? ContentType);
}
