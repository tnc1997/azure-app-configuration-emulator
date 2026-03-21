using Microsoft.AspNetCore.Authentication;

namespace AzureAppConfigurationEmulator.Authentication.Hmac;

public class HmacOptions : AuthenticationSchemeOptions
{
    public string Credential { get; set; } = null!;

    public string Secret { get; set; } = null!;
}
