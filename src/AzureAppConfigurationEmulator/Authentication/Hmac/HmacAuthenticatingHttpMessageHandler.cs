using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;

namespace AzureAppConfigurationEmulator.Authentication.Hmac;

public class HmacAuthenticatingHttpMessageHandler(string credential, string secret) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        using var activity = Telemetry.ActivitySource.StartActivity($"{nameof(HmacAuthenticatingHttpMessageHandler)}.{nameof(SendAsync)}");

        var contentHash = await ComputeContentHash(request, cancellationToken);
        activity?.SetTag(Telemetry.HeaderContentHash, contentHash);

        var date = DateTimeOffset.UtcNow;
        activity?.SetTag(Telemetry.HeaderDate, date);

        request.Headers.Authorization = GetAuthenticationHeaderValue(request, contentHash, date);
        request.Headers.Date = date;
        request.Headers.Add("x-ms-content-sha256", contentHash);

        return await base.SendAsync(request, cancellationToken);
    }

    private static async Task<string> ComputeContentHash(
        HttpRequestMessage request,
        CancellationToken cancellationToken = default)
    {
        using var activity = Telemetry.ActivitySource.StartActivity($"{nameof(HmacAuthenticatingHttpMessageHandler)}.{nameof(ComputeContentHash)}");

        using var stream = new MemoryStream();

        if (request.Content is not null)
        {
            await request.Content.CopyToAsync(stream, cancellationToken);

            stream.Position = 0;
        }

        using var sha256 = SHA256.Create();

        return Convert.ToBase64String(await sha256.ComputeHashAsync(stream, cancellationToken));
    }

    private string ComputeHash(string value)
    {
        using var activity = Telemetry.ActivitySource.StartActivity($"{nameof(HmacAuthenticatingHttpMessageHandler)}.{nameof(ComputeHash)}");

        using var hmac = new HMACSHA256(Convert.FromBase64String(secret));

        return Convert.ToBase64String(hmac.ComputeHash(Encoding.ASCII.GetBytes(value)));
    }

    private AuthenticationHeaderValue GetAuthenticationHeaderValue(
        HttpRequestMessage request,
        string contentHash,
        DateTimeOffset date)
    {
        using var activity = Telemetry.ActivitySource.StartActivity($"{nameof(HmacAuthenticatingHttpMessageHandler)}.{nameof(GetAuthenticationHeaderValue)}");

        const string signedHeaders = "date;host;x-ms-content-sha256";
        activity?.SetTag(Telemetry.HmacSignedHeaders, signedHeaders);

        var stringToSign = $"{request.Method.Method.ToUpper()}\n{request.RequestUri?.PathAndQuery}\n{date:R};{request.RequestUri?.Authority};{contentHash}";
        activity?.SetTag(Telemetry.HmacStringToSign, stringToSign);

        var signature = ComputeHash(stringToSign);
        activity?.SetTag(Telemetry.HmacSignature, signature);

        return new AuthenticationHeaderValue("HMAC-SHA256", $"Credential={credential}&SignedHeaders={signedHeaders}&Signature={signature}");
    }
}
