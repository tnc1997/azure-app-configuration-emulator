using Microsoft.AspNetCore.Authentication;

namespace AzureAppConfigurationEmulator.Authentication.Hmac;

public class HmacOptions : AuthenticationSchemeOptions
{
    public string Credential { get; set; } = default!;

    public string Secret { get; set; } = default!;
}
