#!/bin/bash

set -e

./keygen.sh && dotnet AzureAppConfigurationEmulator.dll
