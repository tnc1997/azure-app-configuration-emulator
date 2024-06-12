# Emulator for Azure App Configuration

Please note that Emulator for Azure App Configuration is unofficial and not endorsed by Microsoft.

## Getting Started

```shell
docker build -f ./src/AzureAppConfigurationEmulator/Dockerfile -t azure-app-configuration-emulator .
docker run -p 8080:8080 azure-app-configuration-emulator
```

## Authentication

The emulator supports HMAC authentication and Microsoft Entra ID authentication.

### HMAC

The credential and secret may be overridden using the environment variables `Authentication__Schemes__Hmac__Credential` and `Authentication__Schemes__Hmac__Secret` respectively.

```yaml
services:
  azure-app-configuration-emulator:
    build:
      context: https://github.com/tnc1997/azure-app-configuration-emulator.git
      dockerfile: ./src/AzureAppConfigurationEmulator/Dockerfile
    environment:
      - Authentication__Schemes__Hmac__Credential=xyz
      - Authentication__Schemes__Hmac__Secret=c2VjcmV0
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
    build:
      context: https://github.com/tnc1997/azure-app-configuration-emulator.git
      dockerfile: ./src/AzureAppConfigurationEmulator/Dockerfile
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

HMAC authentication is recommended because it does not require a Microsoft Entra tenant and an Azure App Configuration resource.

1. [Register an application](https://learn.microsoft.com/entra/identity-platform/quickstart-register-app) within the Microsoft Entra tenant.
    1. On the Overview page, in the Essentials accordion, copy the following values:
        * Application (client) ID
        * Directory (tenant) ID
    2. On the Certificates & secrets page, in the Client secrets tab, add a client secret.
2. [Create an Azure App Configuration resource](https://learn.microsoft.com/azure/azure-app-configuration/quickstart-azure-app-configuration-create) to be emulated.
    1. On the Overview page, in the Essentials accordion, copy the following values:
        * Endpoint
    2. On the Access control (IAM) page, add a role assignment.
        1. In the Role tab, select the App Configuration Data Owner role.
        2. In the Members tab, assign access to the registered application.
3. [Generate a self-signed certificate](#ssl--tls) with the `<endpoint>` as the [Subject Alternative Name](https://wikipedia.org/wiki/Subject_Alternative_Name).

The metadata address must be set using the environment variable `Authentication__Schemes__MicrosoftEntraId__MetadataAddress`.

```yaml
services:
  azure-app-configuration-emulator:
    build:
      context: https://github.com/tnc1997/azure-app-configuration-emulator.git
      dockerfile: ./src/AzureAppConfigurationEmulator/Dockerfile
    environment:
      - ASPNETCORE_HTTP_PORTS=80
      - ASPNETCORE_HTTPS_PORTS=443
      - Authentication__Schemes__MicrosoftEntraId__MetadataAddress=https://login.microsoftonline.com/<tenant-id>/v2.0/.well-known/openid-configuration
    networks:
      default:
        aliases:
          - <endpoint>
    volumes:
      - ./emulator.crt:/usr/local/share/azureappconfigurationemulator/emulator.crt:ro
      - ./emulator.key:/usr/local/share/azureappconfigurationemulator/emulator.key:ro
```

The valid audience should be overriden using the environment variable `Authentication__Schemes__MicrosoftEntraId__ValidAudience`.

```yaml
services:
  azure-app-configuration-emulator:
    build:
      context: https://github.com/tnc1997/azure-app-configuration-emulator.git
      dockerfile: ./src/AzureAppConfigurationEmulator/Dockerfile
    environment:
      - ASPNETCORE_HTTP_PORTS=80
      - ASPNETCORE_HTTPS_PORTS=443
      - Authentication__Schemes__MicrosoftEntraId__MetadataAddress=https://login.microsoftonline.com/<tenant-id>/.well-known/openid-configuration
      - Authentication__Schemes__MicrosoftEntraId__ValidAudience=https://<endpoint>
    networks:
      default:
        aliases:
          - <endpoint>
    volumes:
      - ./emulator.crt:/usr/local/share/azureappconfigurationemulator/emulator.crt:ro
      - ./emulator.key:/usr/local/share/azureappconfigurationemulator/emulator.key:ro
```

#### .NET

The client may authenticate requests using the Microsoft Entra tenant.

```csharp
using Azure.Data.AppConfiguration;
using Azure.Identity;

var tenantId = Environment.GetEnvironmentVariable("Authentication__Schemes__MicrosoftEntraId__TenantId");
var clientId = Environment.GetEnvironmentVariable("Authentication__Schemes__MicrosoftEntraId__ClientId");
var clientSecret = Environment.GetEnvironmentVariable("Authentication__Schemes__MicrosoftEntraId__ClientSecret");
var credential = new ClientSecretCredential(tenantId, clientId, clientSecret);

var endpoint = Environment.GetEnvironmentVariable("Endpoints__AzureAppConfiguration");
var client = new ConfigurationClient(new Uri(endpoint), credential);

var setting = new ConfigurationSetting("AzureAppConfigurationEmulator", "Hello World");
await client.SetConfigurationSettingAsync(setting);
```

```yaml
services:
  azure-app-configuration-emulator:
    build:
      context: https://github.com/tnc1997/azure-app-configuration-emulator.git
      dockerfile: ./src/AzureAppConfigurationEmulator/Dockerfile
    environment:
      - ASPNETCORE_HTTP_PORTS=80
      - ASPNETCORE_HTTPS_PORTS=443
      - Authentication__Schemes__MicrosoftEntraId__MetadataAddress=https://login.microsoftonline.com/<tenant-id>/.well-known/openid-configuration
      - Authentication__Schemes__MicrosoftEntraId__ValidAudience=https://<endpoint>
    networks:
      default:
        aliases:
          - <endpoint>
    volumes:
      - ./emulator.crt:/usr/local/share/azureappconfigurationemulator/emulator.crt:ro
      - ./emulator.key:/usr/local/share/azureappconfigurationemulator/emulator.key:ro
  console-application:
    build:
      context: .
      dockerfile: ./ConsoleApplication/Dockerfile
    depends_on:
      - azure-app-configuration-emulator
    entrypoint: /bin/sh -c "update-ca-certificates && dotnet ConsoleApplication.dll"
    environment:
      - Authentication__Schemes__MicrosoftEntraId__ClientId=<client-id>
      - Authentication__Schemes__MicrosoftEntraId__ClientSecret=<client-secret>
      - Authentication__Schemes__MicrosoftEntraId__TenantId=<tenant-id>
      - Endpoints__AzureAppConfiguration=https://<endpoint>
    volumes:
      - ./emulator.crt:/usr/local/share/ca-certificates/emulator.crt:ro
```

#### Postman

The access token may be obtained using the following configuration:

| Configuration    |                                                                   |
|------------------|-------------------------------------------------------------------|
| Auth Type        | OAuth 2.0                                                         |
| Grant Type       | Client Credentials                                                |
| Access Token URL | `https://login.microsoftonline.com/<tenant-id>/oauth2/v2.0/token` |
| Client ID        | `<client-id>`                                                     |
| Client Secret    | `<client-secret>`                                                 |
| Scope            | `https://<endpoint>/.default`                                     |

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
    build:
      context: https://github.com/tnc1997/azure-app-configuration-emulator.git
      dockerfile: ./src/AzureAppConfigurationEmulator/Dockerfile
    volumes:
      - azure-app-configuration-emulator:/var/lib/azureappconfigurationemulator
volumes:
  azure-app-configuration-emulator:
```

The connection string for the database may be overridden using the environment variable `ConnectionStrings__DefaultConnection`.

```yaml
services:
  azure-app-configuration-emulator:
    build:
      context: https://github.com/tnc1997/azure-app-configuration-emulator.git
      dockerfile: ./src/AzureAppConfigurationEmulator/Dockerfile
    environment:
      - ConnectionStrings__DefaultConnection=Data Source=/var/lib/azureappconfigurationemulator/emulator.db
```

The database may be seeded with data and mounted into the container however care should be taken to ensure that the database has the required schema.

```yaml
services:
  azure-app-configuration-emulator:
    build:
      context: https://github.com/tnc1997/azure-app-configuration-emulator.git
      dockerfile: ./src/AzureAppConfigurationEmulator/Dockerfile
    volumes:
      - ./emulator.db:/var/lib/azureappconfigurationemulator/emulator.db
```

## Messaging

The emulator integrates with Azure Event Grid to publish events using the [Event Grid event schema](https://learn.microsoft.com/en-us/azure/event-grid/custom-topics#event-grid-event-schema) when configuration settings are deleted or modified.

The endpoint and key for the [Event Grid Topic](https://learn.microsoft.com/en-us/azure/event-grid/custom-topics) may be set using the environment variables `Messaging__EventGridTopics__xyz__Endpoint` and `Messaging__EventGridTopics__xyz__Credential__Key` respectively where `xyz` is an arbitrary name.

```yaml
services:
  azure-app-configuration-emulator:
    build:
      context: https://github.com/tnc1997/azure-app-configuration-emulator.git
      dockerfile: ./src/AzureAppConfigurationEmulator/Dockerfile
    environment:
      - Messaging__EventGridTopics__Contoso__Credential__Key=a2V5
      - Messaging__EventGridTopics__Contoso__Endpoint=https://contoso.uksouth-1.eventgrid.azure.net/api/events
```

## Observability

The emulator integrates with OpenTelemetry to provide metrics and traces.

The endpoint for the [OpenTelemetry Protocol (OTLP) Exporter](https://opentelemetry.io/docs/specs/otel/protocol/exporter) may be overridden using the environment variable `OTEL_EXPORTER_OTLP_ENDPOINT`.

```yaml
services:
  azure-app-configuration-emulator:
    build:
      context: https://github.com/tnc1997/azure-app-configuration-emulator.git
      dockerfile: ./src/AzureAppConfigurationEmulator/Dockerfile
    depends_on:
      - opentelemetry-collector
    environment:
      - OTEL_EXPORTER_OTLP_ENDPOINT=http://opentelemetry-collector:4317
  opentelemetry-collector:
    image: otel/opentelemetry-collector-contrib
```

## SSL / TLS

The emulator may be configured to serve requests over HTTPS using a [self-signed certificate](https://wikipedia.org/wiki/self-signed_certificate).

```shell
openssl req -x509 -out ./emulator.crt -keyout ./emulator.key -newkey rsa:2048 -nodes -sha256 -subj '/CN=azure-app-configuration-emulator' -addext 'subjectAltName=DNS:azure-app-configuration-emulator'
```

The port for HTTPS must be set using the environment variable [`ASPNETCORE_HTTPS_PORTS`](https://learn.microsoft.com/aspnet/core/security/enforcing-ssl#port-configuration).

```yaml
services:
  azure-app-configuration-emulator:
    build:
      context: https://github.com/tnc1997/azure-app-configuration-emulator.git
      dockerfile: ./src/AzureAppConfigurationEmulator/Dockerfile
    environment:
      - ASPNETCORE_HTTP_PORTS=8080
      - ASPNETCORE_HTTPS_PORTS=8081
    volumes:
      - ./emulator.crt:/usr/local/share/azureappconfigurationemulator/emulator.crt:ro
      - ./emulator.key:/usr/local/share/azureappconfigurationemulator/emulator.key:ro
```
