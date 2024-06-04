using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace AzureAppConfigurationEmulator.Authentication.Hmac;

public class HmacConfigureOptions(IAuthenticationConfigurationProvider configurationProvider) : IConfigureNamedOptions<HmacOptions>
{
    public void Configure(HmacOptions options)
    {
        Configure(Options.DefaultName, options);
    }

    public void Configure(string? name, HmacOptions options)
    {
        if (string.IsNullOrEmpty(name))
        {
            return;
        }

        var section = configurationProvider.GetSchemeConfiguration(name);

        if (section == null || section.GetChildren().Count() == 0)
        {
            return;
        }

        options.Credential = section.GetValue<string>(nameof(options.Credential)) ?? options.Credential;
        options.Secret = section.GetValue<string>(nameof(options.Secret)) ?? options.Secret;
    }
}
