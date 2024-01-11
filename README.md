# Emulator for Azure App Configuration

Please note that Emulator for Azure App Configuration is unofficial and not endorsed by Microsoft.

## Getting Started

```shell
docker-compose up -d
```

(Leave off the `-d` to run in your current terminal instead of in background/detached mode.)

### Local development

1. Run `./keygen.sh` to generate the HTTPS keypair (Linux/macOS/WSL; Windows instructions TODO)
2. Run `dotnet run` from the `src/AzureAppConfigurationEmulator` folder, or start the app from your IDE

## Authentication

### Postman

Add the following code as a Pre-request Script in Postman to authenticate with the emulator:

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

### Key Values

| Operation                |     |
|--------------------------|-----|
| Get                      | ✔️  |
| Get (Conditionally)      | ✔️  |
| List                     | ✔️  |
| List (Pagination)        | ❌   |
| List (Filtering)         | ✔️  |
| List (Select Fields)     | ❌   |
| List (Time-Based Access) | ❌   |
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
