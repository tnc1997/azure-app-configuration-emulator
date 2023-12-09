using Microsoft.AspNetCore.Authentication;

namespace AzureAppConfigurationEmulator.Authentication;

public class HmacOptions : AuthenticationSchemeOptions
{
    public string Credential { get; set; } = default!;

    public string Secret { get; set; } = default!;
}
