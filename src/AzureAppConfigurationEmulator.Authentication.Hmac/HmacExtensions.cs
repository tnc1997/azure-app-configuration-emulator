using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace AzureAppConfigurationEmulator.Authentication.Hmac;

public static class HmacExtensions
{
    /// <summary>
    /// Enables HMAC authentication using the default scheme <see cref="HmacDefaults.AuthenticationScheme"/>.
    /// <para>
    /// HMAC authentication performs authentication by extracting and validating parameters from the <c>Authorization</c> request header.
    /// </para>
    /// </summary>
    /// <param name="builder">The <see cref="AuthenticationBuilder"/>.</param>
    /// <returns>A reference to <paramref name="builder"/> after the operation has completed.</returns>
    public static AuthenticationBuilder AddHmac(this AuthenticationBuilder builder)
    {
        return builder.AddHmac(HmacDefaults.AuthenticationScheme, _ => { });
    }
    
    /// <summary>
    /// Enables HMAC authentication using a pre-defined scheme.
    /// <para>
    /// HMAC authentication performs authentication by extracting and validating parameters from the <c>Authorization</c> request header.
    /// </para>
    /// </summary>
    /// <param name="builder">The <see cref="AuthenticationBuilder"/>.</param>
    /// <param name="authenticationScheme">The authentication scheme.</param>
    /// <returns>A reference to <paramref name="builder"/> after the operation has completed.</returns>
    public static AuthenticationBuilder AddHmac(this AuthenticationBuilder builder, string authenticationScheme)
    {
        return builder.AddHmac(authenticationScheme, _ => { });
    }

    /// <summary>
    /// Enables HMAC authentication using the default scheme <see cref="HmacDefaults.AuthenticationScheme"/>.
    /// <para>
    /// HMAC authentication performs authentication by extracting and validating parameters from the <c>Authorization</c> request header.
    /// </para>
    /// </summary>
    /// <param name="builder">The <see cref="AuthenticationBuilder"/>.</param>
    /// <param name="configureOptions">A delegate that allows configuring <see cref="HmacOptions"/>.</param>
    /// <returns>A reference to <paramref name="builder"/> after the operation has completed.</returns>
    public static AuthenticationBuilder AddHmac(this AuthenticationBuilder builder, Action<HmacOptions> configureOptions)
    {
        return builder.AddHmac(HmacDefaults.AuthenticationScheme, configureOptions);
    }

    /// <summary>
    /// Enables HMAC authentication using the specified scheme.
    /// <para>
    /// HMAC authentication performs authentication by extracting and validating parameters from the <c>Authorization</c> request header.
    /// </para>
    /// </summary>
    /// <param name="builder">The <see cref="AuthenticationBuilder"/>.</param>
    /// <param name="authenticationScheme">The authentication scheme.</param>
    /// <param name="configureOptions">A delegate that allows configuring <see cref="HmacOptions"/>.</param>
    /// <returns>A reference to <paramref name="builder"/> after the operation has completed.</returns>
    public static AuthenticationBuilder AddHmac(this AuthenticationBuilder builder, string authenticationScheme, Action<HmacOptions> configureOptions)
    {
        return builder.AddHmac(authenticationScheme, null, configureOptions);
    }

    /// <summary>
    /// Enables HMAC authentication using the specified scheme.
    /// <para>
    /// HMAC authentication performs authentication by extracting and validating parameters from the <c>Authorization</c> request header.
    /// </para>
    /// </summary>
    /// <param name="builder">The <see cref="AuthenticationBuilder"/>.</param>
    /// <param name="authenticationScheme">The authentication scheme.</param>
    /// <param name="displayName">The display name for the authentication handler.</param>
    /// <param name="configureOptions">A delegate that allows configuring <see cref="HmacOptions"/>.</param>
    /// <returns>A reference to <paramref name="builder"/> after the operation has completed.</returns>
    public static AuthenticationBuilder AddHmac(this AuthenticationBuilder builder, string authenticationScheme, string? displayName, Action<HmacOptions> configureOptions)
    {
        builder.Services.TryAddSingleton<IConfigureOptions<HmacOptions>, HmacConfigureOptions>();

        return builder.AddScheme<HmacOptions, HmacHandler>(authenticationScheme, displayName, configureOptions);
    }
}
