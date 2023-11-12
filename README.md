# Emulator for Azure App Configuration

Please note that Emulator for Azure App Configuration is unofficial and not endorsed by Microsoft.

## Getting Started

```shell
openssl req -x509 -out ./emulator.crt -keyout ./emulator.key -newkey rsa:2048 -nodes -sha256 -subj '/CN=localhost' -addext 'subjectAltName=DNS:localhost'
docker build -f ./src/AzureAppConfigurationEmulator/Dockerfile -t azure-app-configuration-emulator .
docker run -e ASPNETCORE_HTTP_PORTS=8080 -e ASPNETCORE_HTTPS_PORTS=8081 -p 8080:8080 -p 8081:8081 -v ./emulator.crt:/usr/local/share/azureappconfigurationemulator/emulator.crt:ro -v ./emulator.key:/usr/local/share/azureappconfigurationemulator/emulator.key:ro azure-app-configuration-emulator
```

## Compatibility

### Key Values

| Operation                |     |
|--------------------------|-----|
| Get                      | ✔️  |
| Get (Conditionally)      | ❌   |
| List                     | ✔️  |
| List (Pagination)        | ❌   |
| List (Filtering)         | ✔️  |
| List (Select Fields)     | ❌   |
| List (Time-Based Access) | ❌   |
| Set                      | ✔️  |
| Set (Conditionally)      | ❌   |
| Delete                   | ✔️  |
| Delete (Conditionally)   | ❌   |

### Keys

| Operation                |    |
|--------------------------|----|
| List                     | ✔️ |
| List (Pagination)        | ❌  |
| List (Filtering)         | ✔️ |
| List (Select Fields)     | ❌  |
| List (Time-Based Access) | ❌  |

### Labels

| Operation                |    |
|--------------------------|----|
| List                     | ✔️ |
| List (Pagination)        | ❌  |
| List (Filtering)         | ✔️ |
| List (Select Fields)     | ❌  |
| List (Time-Based Access) | ❌  |

### Locks

| Operation              |    |
|------------------------|----|
| Lock                   | ✔️ |
| Lock (Conditionally)   | ❌  |
| Unlock                 | ✔️ |
| Unlock (Conditionally) | ❌  |

### Revisions

| Operation                |   |
|--------------------------|---|
| List                     | ❌ |
| List (Pagination)        | ❌ |
| List (Range)             | ❌ |
| List (Filtering)         | ❌ |
| List (Select Fields)     | ❌ |
| List (Time-Based Access) | ❌ |
