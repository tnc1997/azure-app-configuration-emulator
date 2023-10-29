# Emulator for Azure App Configuration

Please note that Emulator for Azure App Configuration is unofficial and not endorsed by Microsoft.

## Getting Started

```shell
docker build -f ./src/AzureAppConfigurationEmulator/Dockerfile -t azure-app-configuration-emulator .
docker run -p 8080:8080 azure-app-configuration-emulator
```

## Compatibility

### Key Values

| Operation                |     |
|--------------------------|-----|
| Get                      | ❌   |
| Get (Conditionally)      | ❌   |
| List                     | ✔️  |
| List (Pagination)        | ❌   |
| List (Filtering)         | ✔️  |
| List (Select Fields)     | ❌   |
| List (Time-Based Access) | ❌   |
| Set                      | ✔️  |
| Set (Conditionally)      | ❌   |
| Delete                   | ❌   |
| Delete (Conditionally)   | ❌   |

### Keys

| Operation                |   |
|--------------------------|---|
| List                     | ❌ |
| List (Pagination)        | ❌ |
| List (Filtering)         | ❌ |
| List (Select Fields)     | ❌ |
| List (Time-Based Access) | ❌ |

### Labels

| Operation                |   |
|--------------------------|---|
| List                     | ❌ |
| List (Pagination)        | ❌ |
| List (Filtering)         | ❌ |
| List (Select Fields)     | ❌ |
| List (Time-Based Access) | ❌ |

### Locks

| Operation |   |
|-----------|---|
| Lock      | ❌ |
| Unlock    | ❌ |

### Revisions

| Operation                |   |
|--------------------------|---|
| List                     | ❌ |
| List (Pagination)        | ❌ |
| List (Range)             | ❌ |
| List (Filtering)         | ❌ |
| List (Select Fields)     | ❌ |
| List (Time-Based Access) | ❌ |
