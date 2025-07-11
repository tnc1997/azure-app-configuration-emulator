# Emulator for Azure App Configuration

![Docker Pulls](https://img.shields.io/docker/pulls/tnc1997/azure-app-configuration-emulator?link=https%3A%2F%2Fhub.docker.com%2Fr%2Ftnc1997%2Fazure-app-configuration-emulator) ![Docker Stars](https://img.shields.io/docker/stars/tnc1997/azure-app-configuration-emulator?link=https%3A%2F%2Fhub.docker.com%2Fr%2Ftnc1997%2Fazure-app-configuration-emulator)

Please note that Emulator for Azure App Configuration is unofficial and not endorsed by Microsoft.

## Getting Started

```shell
docker pull tnc1997/azure-app-configuration-emulator
docker run -p 8080:8080 tnc1997/azure-app-configuration-emulator
```

## Authentication

The emulator supports HMAC authentication and Microsoft Entra ID authentication.

### HMAC

The credential and secret may be overridden using the environment variables `Authentication__Schemes__Hmac__Credential` and `Authentication__Schemes__Hmac__Secret` respectively.

```yaml
services:
  azure-app-configuration-emulator:
    environment:
      - Authentication__Schemes__Hmac__Credential=xyz
      - Authentication__Schemes__Hmac__Secret=c2VjcmV0
    image: tnc1997/azure-app-configuration-emulator
```

#### .NET

The client may authenticate requests using the connection string for the emulator.

```csharp
using Azure.Data.AppConfiguration;

var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__AzureAppConfiguration");
var client = new ConfigurationClient(connectionString);

var setting = new ConfigurationSetting("AzureAppConfigurationEmulator", "Hello World");
await client.SetConfigurationSettingAsync(setting);
```

```yaml
services:
  azure-app-configuration-emulator:
    image: tnc1997/azure-app-configuration-emulator
  console-application:
    build:
      context: .
      dockerfile: ./ConsoleApplication/Dockerfile
    depends_on:
      - azure-app-configuration-emulator
    environment:
      - ConnectionStrings__AzureAppConfiguration=Endpoint=http://azure-app-configuration-emulator:8080;Id=abcd;Secret=c2VjcmV0;
```

#### Postman

The authentication related headers may be generated using the following script:

```javascript
const credential = "abcd";
const secret = "c2VjcmV0";

const date = new Date().toUTCString();
const contentHash = CryptoJS.SHA256(CryptoJS.enc.Utf8.parse(pm.request.body.toString())).toString(CryptoJS.enc.Base64);

const signedHeaders = "x-ms-date;Host;x-ms-content-sha256";
const stringToSign = `${pm.request.method}\n${pm.request.url.getPathWithQuery()}\n${date};${pm.request.url.getRemote()};${contentHash}`;
const signature = CryptoJS.HmacSHA256(CryptoJS.enc.Utf8.parse(stringToSign), CryptoJS.enc.Base64.parse(secret)).toString(CryptoJS.enc.Base64);

pm.request.headers.upsert(`x-ms-date: ${date}`);
pm.request.headers.upsert(`x-ms-content-sha256: ${contentHash}`);
pm.request.headers.upsert(`Authorization: HMAC-SHA256 Credential=${credential}&SignedHeaders=${signedHeaders}&Signature=${signature}`);
```

### Microsoft Entra ID

Microsoft Entra ID authentication allows you to simulate an Azure based production environment using a [Managed Identity](https://learn.microsoft.com/en-us/azure/azure-app-configuration/howto-integrate-azure-managed-service-identity).

[Assumed Identity](https://github.com/nagyesta/assumed-identity) is a simple test double simulating how Azure Instance Metadata Service (IMDS) is handling Managed Identity tokens.

The metadata address must be set using the environment variable `Authentication__Schemes__MicrosoftEntraId__MetadataAddress`.

```yaml
services:
  assumed-identity:
    image: nagyesta/assumed-identity
  azure-app-configuration-emulator:
    depends_on:
      - assumed-identity
    environment:
      - ASPNETCORE_HTTP_PORTS=8080
      - ASPNETCORE_HTTPS_PORTS=8081
      - Authentication__Schemes__MicrosoftEntraId__MetadataAddress=http://assumed-identity/metadata/identity/.well-known/openid-configuration
      - Authentication__Schemes__MicrosoftEntraId__RequireHttpsMetadata=false
      - Kestrel__Certificates__Default__Path=/usr/local/share/azureappconfigurationemulator/emulator.crt
      - Kestrel__Certificates__Default__KeyPath=/usr/local/share/azureappconfigurationemulator/emulator.key
    image: tnc1997/azure-app-configuration-emulator
    volumes:
      - ./emulator.crt:/usr/local/share/azureappconfigurationemulator/emulator.crt:ro
      - ./emulator.key:/usr/local/share/azureappconfigurationemulator/emulator.key:ro
```

#### .NET

The client may authenticate requests using the Managed Identity.

```csharp
using Azure.Data.AppConfiguration;
using Azure.Identity;

var endpoint = Environment.GetEnvironmentVariable("Endpoints__AzureAppConfiguration");
var credential = new ManagedIdentityCredential();
var client = new ConfigurationClient(new Uri(endpoint), credential);

var setting = new ConfigurationSetting("AzureAppConfigurationEmulator", "Hello World");
await client.SetConfigurationSettingAsync(setting);
```

```yaml
services:
  assumed-identity:
    image: nagyesta/assumed-identity
  azure-app-configuration-emulator:
    depends_on:
      - assumed-identity
    environment:
      - ASPNETCORE_HTTP_PORTS=8080
      - ASPNETCORE_HTTPS_PORTS=8081
      - Authentication__Schemes__MicrosoftEntraId__MetadataAddress=http://assumed-identity/metadata/identity/.well-known/openid-configuration
      - Authentication__Schemes__MicrosoftEntraId__RequireHttpsMetadata=false
      - Kestrel__Certificates__Default__Path=/usr/local/share/azureappconfigurationemulator/emulator.crt
      - Kestrel__Certificates__Default__KeyPath=/usr/local/share/azureappconfigurationemulator/emulator.key
    image: tnc1997/azure-app-configuration-emulator
    volumes:
      - ./emulator.crt:/usr/local/share/azureappconfigurationemulator/emulator.crt:ro
      - ./emulator.key:/usr/local/share/azureappconfigurationemulator/emulator.key:ro
  console-application:
    build:
      context: .
      dockerfile: ./ConsoleApplication/Dockerfile
    depends_on:
      - assumed-identity
      - azure-app-configuration-emulator
    entrypoint: /bin/sh -c "update-ca-certificates && dotnet ConsoleApplication.dll"
    environment:
      - Endpoints__AzureAppConfiguration=https://azure-app-configuration-emulator:8081
    volumes:
      - ./emulator.crt:/usr/local/share/ca-certificates/emulator.crt:ro
```

## Compatibility

The emulator is compatible with the following operations:

### Key Values

| Operation                |     |
|--------------------------|-----|
| Get                      | ✔️  |
| Get (Conditionally)      | ✔️  |
| Get (Select Fields)      | ✔️  |
| Get (Time-Based Access)  | ✔️  |
| List                     | ✔️  |
| List (Pagination)        | ❌   |
| List (Filtering)         | ✔️  |
| List (Select Fields)     | ✔️  |
| List (Time-Based Access) | ✔️  |
| Set                      | ✔️  |
| Set (Conditionally)      | ✔️  |
| Delete                   | ✔️  |
| Delete (Conditionally)   | ✔️  |

### Keys

| Operation                |    |
|--------------------------|----|
| List                     | ✔️ |
| List (Pagination)        | ❌  |
| List (Filtering)         | ✔️ |
| List (Select Fields)     | ✔️ |
| List (Time-Based Access) | ✔️ |

### Labels

| Operation                |    |
|--------------------------|----|
| List                     | ✔️ |
| List (Pagination)        | ❌  |
| List (Filtering)         | ✔️ |
| List (Select Fields)     | ✔️ |
| List (Time-Based Access) | ✔️ |

### Locks

| Operation              |    |
|------------------------|----|
| Lock                   | ✔️ |
| Lock (Conditionally)   | ✔️ |
| Unlock                 | ✔️ |
| Unlock (Conditionally) | ✔️ |

### Revisions

| Operation                |   |
|--------------------------|---|
| List                     | ❌ |
| List (Pagination)        | ❌ |
| List (Range)             | ❌ |
| List (Filtering)         | ❌ |
| List (Select Fields)     | ❌ |
| List (Time-Based Access) | ❌ |

## Data

The emulator stores configuration settings in a [SQLite](https://sqlite.org) database.

The data that is generated by the emulator during a session may be persisted between sessions using a [volume](https://docs.docker.com/storage/volumes).

```yaml
services:
  azure-app-configuration-emulator:
    image: tnc1997/azure-app-configuration-emulator
    volumes:
      - azure-app-configuration-emulator:/var/lib/azureappconfigurationemulator
volumes:
  azure-app-configuration-emulator:
```

The connection string for the database may be overridden using the environment variable `ConnectionStrings__DefaultConnection`.

```yaml
services:
  azure-app-configuration-emulator:
    environment:
      - ConnectionStrings__DefaultConnection=Data Source=/var/lib/azureappconfigurationemulator/emulator.db
    image: tnc1997/azure-app-configuration-emulator
```

The database may be seeded with data and mounted into the container however care should be taken to ensure that the database has the required schema.

```yaml
services:
  azure-app-configuration-emulator:
    image: tnc1997/azure-app-configuration-emulator
    volumes:
      - ./emulator.db:/var/lib/azureappconfigurationemulator/emulator.db
```

## Messaging

The emulator integrates with Azure Event Grid to publish events using the [Event Grid event schema](https://learn.microsoft.com/en-us/azure/event-grid/custom-topics#event-grid-event-schema) when configuration settings are deleted or modified.

The endpoint and key for the [Event Grid Topic](https://learn.microsoft.com/en-us/azure/event-grid/custom-topics) may be set using the environment variables `Messaging__EventGridTopics__xyz__Endpoint` and `Messaging__EventGridTopics__xyz__Credential__Key` respectively where `xyz` is an arbitrary name.

```yaml
services:
  azure-app-configuration-emulator:
    environment:
      - Messaging__EventGridTopics__Contoso__Credential__Key=a2V5
      - Messaging__EventGridTopics__Contoso__Endpoint=https://contoso.uksouth-1.eventgrid.azure.net/api/events
    image: tnc1997/azure-app-configuration-emulator
```

## Observability

The emulator integrates with OpenTelemetry to provide metrics and traces.

The endpoint for the [OpenTelemetry Protocol (OTLP) Exporter](https://opentelemetry.io/docs/specs/otel/protocol/exporter) may be overridden using the environment variable `OTEL_EXPORTER_OTLP_ENDPOINT`.

```yaml
services:
  azure-app-configuration-emulator:
    depends_on:
      - opentelemetry-collector
    environment:
      - OTEL_EXPORTER_OTLP_ENDPOINT=http://opentelemetry-collector:4317
    image: tnc1997/azure-app-configuration-emulator
  opentelemetry-collector:
    image: otel/opentelemetry-collector-contrib
```

## SSL / TLS

The emulator may be configured to serve requests over HTTPS using a [self-signed certificate](https://wikipedia.org/wiki/self-signed_certificate).

```shell
openssl req -x509 -out ./emulator.crt -keyout ./emulator.key -newkey rsa:2048 -nodes -sha256 -subj '/CN=azure-app-configuration-emulator' -addext 'subjectAltName=DNS:azure-app-configuration-emulator'
```

The port for HTTPS must be set using the environment variable [`ASPNETCORE_HTTPS_PORTS`](https://learn.microsoft.com/aspnet/core/security/enforcing-ssl#port-configuration).

The paths for the certificate and key must be set using the environment variables `Kestrel__Certificates__Default__Path` and `Kestrel__Certificates__Default__KeyPath` respectively.

The certificate and key must be [mounted](https://docs.docker.com/storage/bind-mounts) into the container at the paths that are set above.

```yaml
services:
  azure-app-configuration-emulator:
    environment:
      - ASPNETCORE_HTTP_PORTS=8080
      - ASPNETCORE_HTTPS_PORTS=8081
      - Kestrel__Certificates__Default__Path=/usr/local/share/azureappconfigurationemulator/emulator.crt
      - Kestrel__Certificates__Default__KeyPath=/usr/local/share/azureappconfigurationemulator/emulator.key
    image: tnc1997/azure-app-configuration-emulator
    volumes:
      - ./emulator.crt:/usr/local/share/azureappconfigurationemulator/emulator.crt:ro
      - ./emulator.key:/usr/local/share/azureappconfigurationemulator/emulator.key:ro
```

## Testcontainers

The emulator integrates with [Testcontainers](https://testcontainers.org) to ease the integration testing of applications that use Azure App Configuration.

```csharp
var container = new ContainerBuilder()
    .WithImage("tnc1997/azure-app-configuration-emulator:1.0")
    .WithPortBinding(8080, true)
    .WithWaitStrategy(Wait.ForUnixContainer().UntilMessageIsLogged("Now listening on"))
    .Build();

await container.StartAsync();

var client = new ConfigurationClient($"Endpoint={new UriBuilder(Uri.UriSchemeHttp, container.Hostname, container.GetMappedPublicPort(8080))};Id=abcd;Secret=c2VjcmV0");

await client.SetConfigurationSettingAsync(nameof(ConfigurationSetting.Key), nameof(ConfigurationSetting.Value));

var response = await client.GetConfigurationSettingAsync(nameof(ConfigurationSetting.Key));
```

**Coming Soon** [testcontainers/testcontainers-dotnet#1198](https://github.com/testcontainers/testcontainers-dotnet/issues/1198)

```csharp
var container = new AzureAppConfigurationBuilder().Build();

await container.StartAsync();

var client = new ConfigurationClient(container.GetConnectionString());

await client.SetConfigurationSettingAsync(nameof(ConfigurationSetting.Key), nameof(ConfigurationSetting.Value));

var response = await client.GetConfigurationSettingAsync(nameof(ConfigurationSetting.Key));
```
