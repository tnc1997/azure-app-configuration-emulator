# Emulator for Azure App Configuration

Please note that Emulator for Azure App Configuration is unofficial and not endorsed by Microsoft.

## Getting Started

```shell
openssl req -x509 -out ./emulator.crt -keyout ./emulator.key -newkey rsa:2048 -nodes -sha256 -subj '/CN=localhost' -addext 'subjectAltName=DNS:localhost'
docker build -f ./src/AzureAppConfigurationEmulator/Dockerfile -t azure-app-configuration-emulator .
docker run -e ASPNETCORE_HTTP_PORTS=8080 -e ASPNETCORE_HTTPS_PORTS=8081 -p 8080:8080 -p 8081:8081 -v ./emulator.crt:/usr/local/share/azureappconfigurationemulator/emulator.crt:ro -v ./emulator.key:/usr/local/share/azureappconfigurationemulator/emulator.key:ro azure-app-configuration-emulator-data-potection-keys:/root/.aspnet/DataProtection-Keys azure-app-configuration-emulator
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
      - ASPNETCORE_HTTP_PORTS=8080
      - Authentication__Schemes__Hmac__Credential=xyz
      - Authentication__Schemes__Hmac__Secret=c2VjcmV0
    ports:
      - "8080:8080"
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

The metadata address must be set using the environment variable `Authentication__Schemes__MicrosoftEntraId__MetadataAddress` where `00000000-0000-0000-0000-000000000000` is the [tenant identifier](https://learn.microsoft.com/entra/fundamentals/how-to-find-tenant).

```yaml
services:
  azure-app-configuration-emulator:
    build:
      context: https://github.com/tnc1997/azure-app-configuration-emulator.git
      dockerfile: ./src/AzureAppConfigurationEmulator/Dockerfile
    environment:
      - ASPNETCORE_HTTP_PORTS=8080
      - Authentication__Schemes__MicrosoftEntraId__MetadataAddress=https://login.microsoftonline.com/00000000-0000-0000-0000-000000000000/v2.0/.well-known/openid-configuration
    ports:
      - "8080:8080"
```

The valid audience may be overriden using the environment variable `Authentication__Schemes__MicrosoftEntraId__ValidAudience`.

```yaml
services:
  azure-app-configuration-emulator:
    build:
      context: https://github.com/tnc1997/azure-app-configuration-emulator.git
      dockerfile: ./src/AzureAppConfigurationEmulator/Dockerfile
    environment:
      - ASPNETCORE_HTTP_PORTS=8080
      - Authentication__Schemes__MicrosoftEntraId__MetadataAddress=https://login.microsoftonline.com/00000000-0000-0000-0000-000000000000/v2.0/.well-known/openid-configuration
      - Authentication__Schemes__MicrosoftEntraId__ValidAudience=https://contoso.azconfig.io
    ports:
      - "8080:8080"
```

## Compatibility

The emulator is compatible with the following operations:

### Key Values

| Operation                |     |
|--------------------------|-----|
| Get                      | ✔️  |
| Get (Conditionally)      | ✔️  |
| List                     | ✔️  |
| List (Pagination)        | ❌   |
| List (Filtering)         | ✔️  |
| List (Select Fields)     | ❌   |
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
| List (Select Fields)     | ❌  |
| List (Time-Based Access) | ✔️ |

### Labels

| Operation                |    |
|--------------------------|----|
| List                     | ✔️ |
| List (Pagination)        | ❌  |
| List (Filtering)         | ✔️ |
| List (Select Fields)     | ❌  |
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
      - ASPNETCORE_HTTP_PORTS=8080
      - Messaging__EventGridTopics__Contoso__Credential__Key=a2V5
      - Messaging__EventGridTopics__Contoso__Endpoint=https://contoso.uksouth-1.eventgrid.azure.net/api/events
    ports:
      - "8080:8080"
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
      - ASPNETCORE_HTTP_PORTS=8080
      - OTEL_EXPORTER_OTLP_ENDPOINT=http://opentelemetry-collector:4317
    ports:
      - "8080:8080"
  opentelemetry-collector:
    image: otel/opentelemetry-collector-contrib
    ports:
      - "4317:4317"
      - "4318:4318"
```
