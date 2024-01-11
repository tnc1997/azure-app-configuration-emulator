using System.Net.Http.Headers;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Options;

namespace AzureAppConfigurationEmulator.Authentication;

public class HmacHandler(IOptionsMonitor<HmacOptions> options, ILoggerFactory logger, UrlEncoder encoder) : AuthenticationHandler<HmacOptions>(options, logger, encoder)
{
    private IEnumerable<string> RequiredParameters { get; } = new List<string>
    {
        AuthenticationParameters.SignedHeaders,
        AuthenticationParameters.Credential,
        AuthenticationParameters.Signature
    };
    
    private IDictionary<string, IEnumerable<string>> RequiredSignedHeaders { get; } = new Dictionary<string, IEnumerable<string>>
    {
        { HeaderNames.Date, new List<string> { HeaderNames.XMsDate, HeaderNames.Date } },
        { HeaderNames.Host, new List<string> { HeaderNames.Host } },
        { HeaderNames.XMsContentSha256, new List<string> { HeaderNames.XMsContentSha256 } }
    };

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        Logger.LogDebug($"Checking if the request headers contains the header '{HeaderNames.Authorization}'.");
        if (!AuthenticationHeaderValue.TryParse(Request.Headers[HeaderNames.Authorization], out var value))
        {
            return AuthenticateResult.NoResult();
        }

        Logger.LogDebug($"Checking if the scheme '{{Scheme}}' equals '{AuthenticationSchemes.HmacSha256}'.", value.Scheme);
        if (!value.Scheme.Equals(AuthenticationSchemes.HmacSha256, StringComparison.OrdinalIgnoreCase))
        {
            return AuthenticateResult.NoResult();
        }

        Logger.LogDebug("Checking if the parameter '{Parameter}' is null or empty.", value.Parameter);
        if (string.IsNullOrEmpty(value.Parameter))
        {
            return AuthenticateResult.NoResult();
        }

        Logger.LogDebug("Splitting the parameter '{Parameter}' on the delimiter '&'.", value.Parameter);
        var parameters = value.Parameter.Split('&').ToDictionary(
            parameter => parameter[..parameter.IndexOf('=')],
            parameter => parameter[(parameter.IndexOf('=') + 1)..],
            StringComparer.OrdinalIgnoreCase);

        using (Logger.BeginScope(parameters))
        {
            foreach (var parameter in RequiredParameters)
            {
                Logger.LogDebug("Checking if the parameters contains the parameter '{Parameter}'.", parameter);
                if (!parameters.ContainsKey(parameter))
                {
                    return AuthenticateResult.Fail($"{parameter} parameter is required");
                }
            }

            foreach (var (header, headers) in RequiredSignedHeaders)
            {
                Logger.LogDebug("Checking if the signed headers parameter contains the header '{Header}'.", header);
                if (!parameters[AuthenticationParameters.SignedHeaders].Split(';').Intersect(headers, StringComparer.OrdinalIgnoreCase).Any())
                {
                    return AuthenticateResult.Fail($"Required signing request header '{header}' not found");
                }
            }

            foreach (var header in parameters[AuthenticationParameters.SignedHeaders].Split(';'))
            {
                Logger.LogDebug("Checking if the request headers contains the signed header '{Header}'.", header);
                if (!Request.Headers.ContainsKey(header))
                {
                    return AuthenticateResult.Fail($"Required signing request header '{header}' not found");
                }
            }

            Logger.LogDebug($"Checking if the request headers contains the header '{HeaderNames.Date}'.");
            if (!DateTimeOffset.TryParse(Request.Headers[HeaderNames.XMsDate], out var date) && !DateTimeOffset.TryParse(Request.Headers[HeaderNames.Date], out date))
            {
                return AuthenticateResult.Fail("Invalid access token date");
            }

            Logger.LogDebug("Checking if the date '{Date}' is more than 15 minutes away from now.", date);
            if (DateTimeOffset.UtcNow - date > TimeSpan.FromMinutes(15))
            {
                return AuthenticateResult.Fail("The access token has expired");
            }

            Logger.LogDebug("Checking if the credential is valid.");
            if (!parameters[AuthenticationParameters.Credential].Equals(Options.Credential, StringComparison.Ordinal))
            {
                return AuthenticateResult.Fail("Invalid credential");
            }

            Logger.LogDebug("Checking if the signature is valid.");
            if (!parameters[AuthenticationParameters.Signature].Equals(GetSignature(parameters[AuthenticationParameters.SignedHeaders]), StringComparison.Ordinal) && !parameters[AuthenticationParameters.Signature].Equals(GetSignature(parameters[AuthenticationParameters.SignedHeaders], false), StringComparison.Ordinal))
            {
                return AuthenticateResult.Fail("Invalid signature");
            }

            try
            {
                Logger.LogDebug("Enabling buffering of the request body.");
                Request.EnableBuffering();

                Logger.LogDebug("Checking if the content hash is valid.");
                if (!Request.Headers[HeaderNames.XMsContentSha256].ToString().Equals(await ComputeContentHashAsync(Request.Body), StringComparison.Ordinal))
                {
                    return AuthenticateResult.Fail("Invalid request content hash");
                }
            }
            finally
            {
                Logger.LogDebug("Resetting the request body position.");
                Request.Body.Position = 0;
            }

            return AuthenticateResult.Success(new AuthenticationTicket(new ClaimsPrincipal(new ClaimsIdentity(Scheme.Name)), Scheme.Name));
        }
    }

    protected override async Task HandleChallengeAsync(AuthenticationProperties properties)
    {
        var result = await HandleAuthenticateOnceSafeAsync();

        Logger.LogDebug("Setting the response status code.");
        Response.StatusCode = 401;

        Logger.LogDebug($"Setting the response header '{HeaderNames.WwwAuthenticate}'.");
        Response.Headers[HeaderNames.WwwAuthenticate] = new AuthenticationHeaderValue(
            AuthenticationSchemes.HmacSha256,
            !string.IsNullOrEmpty(result.Failure?.Message)
                ? $"{AuthenticationParameters.Error}=\"invalid_token\", {AuthenticationParameters.ErrorDescription}=\"{result.Failure.Message}\""
                : null).ToString();
    }

    private async Task<string> ComputeContentHashAsync(Stream content)
    {
        Logger.LogDebug("Creating a new SHA-256 instance.");
        using var sha256 = SHA256.Create();

        Logger.LogDebug("Computing the SHA-256 hash of the content.");
        return Convert.ToBase64String(await sha256.ComputeHashAsync(content));
    }

    private string ComputeHash(string value)
    {
        Logger.LogDebug("Constructing a new HMAC-SHA-256 instance with the secret '{Secret}'.", Options.Secret);
        using var hmac = new HMACSHA256(Convert.FromBase64String(Options.Secret));

        Logger.LogDebug("Computing the HMAC-SHA-256 hash of the value '{Value}'.", value);
        return Convert.ToBase64String(hmac.ComputeHash(Encoding.ASCII.GetBytes(value)));
    }

    private string GetSignature(string signedHeaders, bool shouldIncludePort = true)
    {
        var builder = new StringBuilder();

        var method = Request.Method.ToUpper();
        Logger.LogDebug("Appending the method '{Method}' to the string to sign.", method);
        builder.Append($"{method}\n");

        var target = Context.Features.GetRequiredFeature<IHttpRequestFeature>().RawTarget;
        Logger.LogDebug("Appending the path and query '{PathAndQuery}' to the string to sign.", target);
        builder.Append($"{target}\n");

        var signedHeadersValues = new List<string>();
        Logger.LogDebug("Splitting the signed headers '{Headers}' on the delimiter ';'.", signedHeaders);
        foreach (var signedHeader in signedHeaders.Split(';'))
        {
            Logger.LogDebug("Getting the signed header '{Header}' from the request headers.", signedHeader);
            var signedHeadersValue = Request.Headers[signedHeader].ToString();
            // TODO: Remove the should include port logic when this (https://github.com/Azure/azure-sdk-for-net/issues/41051) issue is closed.
            if (!shouldIncludePort && signedHeader.Equals(HeaderNames.Host, StringComparison.OrdinalIgnoreCase) && signedHeadersValue.Contains(':'))
            {
                Logger.LogDebug("Removing the port from the host request header value.");
                signedHeadersValue = signedHeadersValue[..signedHeadersValue.IndexOf(':')];
            }
            Logger.LogDebug("Adding the signed headers value '{Value}' to the signed headers values.", signedHeadersValue);
            signedHeadersValues.Add(signedHeadersValue);
        }
        Logger.LogDebug("Appending the signed headers values '{Values}' to the string to sign.", signedHeadersValues);
        builder.AppendJoin(';', signedHeadersValues);

        return ComputeHash(builder.ToString());
    }

    private static class AuthenticationParameters
    {
        public const string Credential = "Credential";

        public const string Error = "error";

        public const string ErrorDescription = "error_description";

        public const string Signature = "Signature";

        public const string SignedHeaders = "SignedHeaders";
    }

    private static class AuthenticationSchemes
    {
        public const string HmacSha256 = "HMAC-SHA256";
    }

    private static class HeaderNames
    {
        public const string Authorization = "Authorization";

        public const string Date = "Date";

        public const string Host = "Host";
        
        public const string WwwAuthenticate = "WWW-Authenticate";

        public const string XMsContentSha256 = "x-ms-content-sha256";

        public const string XMsDate = "x-ms-date";
    }
}
