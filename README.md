# Emulator for Azure App Configuration

Please note that Emulator for Azure App Configuration is unofficial and not endorsed by Microsoft.

## Getting Started

```shell
openssl req -x509 -out ./emulator.crt -keyout ./emulator.key -newkey rsa:2048 -nodes -sha256 -subj '/CN=localhost' -addext 'subjectAltName=DNS:localhost'
docker build -f ./src/AzureAppConfigurationEmulator/Dockerfile -t azure-app-configuration-emulator .
docker run -e ASPNETCORE_HTTP_PORTS=8080 -e ASPNETCORE_HTTPS_PORTS=8081 -p 8080:8080 -p 8081:8081 -v ./emulator.crt:/usr/local/share/azureappconfigurationemulator/emulator.crt:ro -v ./emulator.key:/usr/local/share/azureappconfigurationemulator/emulator.key:ro azure-app-configuration-emulator-data-potection-keys:/root/.aspnet/DataProtection-Keys azure-app-configuration-emulator
```

## Authentication

The emulator supports HMAC authentication but does not support Microsoft Entra ID authentication.

The credential and secret can be overridden using the environment variables `Authentication__Schemes__Hmac__Credential` and `Authentication__Schemes__Hmac__Secret` respectively.

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

### Postman

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

The endpoint and key for the [Event Grid Topic](https://learn.microsoft.com/en-us/azure/event-grid/custom-topics) can be set using the environment variables `Messaging__EventGridTopics__xyz__Endpoint` and `Messaging__EventGridTopics__xyz__Credential__Key` respectively where `xyz` is an arbitrary name.

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

The endpoint for the [OpenTelemetry Protocol (OTLP) Exporter](https://opentelemetry.io/docs/specs/otel/protocol/exporter) can be overridden using the environment variable `OTEL_EXPORTER_OTLP_ENDPOINT`.

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
