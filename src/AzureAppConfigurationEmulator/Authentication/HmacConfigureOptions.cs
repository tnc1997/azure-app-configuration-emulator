using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace AzureAppConfigurationEmulator.Authentication;

public class HmacConfigureOptions(IAuthenticationConfigurationProvider configurationProvider) : IConfigureNamedOptions<HmacOptions>
{
    private IAuthenticationConfigurationProvider ConfigurationProvider { get; } = configurationProvider;

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

        var section = ConfigurationProvider.GetSchemeConfiguration(name);

        if (section == null || section.GetChildren().Count() == 0)
        {
            return;
        }

        options.Credential = section.GetValue<string>(nameof(options.Credential)) ?? options.Credential;
        options.Secret = section.GetValue<string>(nameof(options.Secret)) ?? options.Secret;
    }
}
