#!/bin/bash

set -e

crt_file="${SSL_CERTIFICATE_CERT_PATH:-./src/AzureAppConfigurationEmulator/data/emulator.crt}"
key_file="${SSL_CERTIFICATE_KEY_PATH:-./src/AzureAppConfigurationEmulator/data/emulator.key}"

echo "Cert file: $crt_file"
echo "Key file: $key_file"

if [ -f "$crt_file" ] && [ -f "$key_file" ]; then
    echo "Certificate and key already exist, skipping generation"
    exit 0
fi

openssl req -x509 -out "$crt_file" -keyout "$key_file" -newkey rsa:2048 -nodes -sha256 -subj '/CN=localhost' -addext 'subjectAltName=DNS:localhost'

echo "Certificate and key generated successfully"
