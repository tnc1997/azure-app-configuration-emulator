FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY "./src/AzureAppConfigurationEmulator/AzureAppConfigurationEmulator.csproj" "./src/AzureAppConfigurationEmulator/AzureAppConfigurationEmulator.csproj"
COPY "./src/AzureAppConfigurationEmulator.Authentication.Hmac/AzureAppConfigurationEmulator.Authentication.Hmac.csproj" "./src/AzureAppConfigurationEmulator.Authentication.Hmac/AzureAppConfigurationEmulator.Authentication.Hmac.csproj"
COPY "./src/AzureAppConfigurationEmulator.Common/AzureAppConfigurationEmulator.Common.csproj" "./src/AzureAppConfigurationEmulator.Common/AzureAppConfigurationEmulator.Common.csproj"
COPY "./src/AzureAppConfigurationEmulator.ConfigurationSettings/AzureAppConfigurationEmulator.ConfigurationSettings.csproj" "./src/AzureAppConfigurationEmulator.ConfigurationSettings/AzureAppConfigurationEmulator.ConfigurationSettings.csproj"
COPY "./src/AzureAppConfigurationEmulator.ConfigurationSettings.Messaging.EventGrid/AzureAppConfigurationEmulator.ConfigurationSettings.Messaging.EventGrid.csproj" "./src/AzureAppConfigurationEmulator.ConfigurationSettings.Messaging.EventGrid/AzureAppConfigurationEmulator.ConfigurationSettings.Messaging.EventGrid.csproj"
COPY "./src/AzureAppConfigurationEmulator.Data.Abstractions/AzureAppConfigurationEmulator.Data.Abstractions.csproj" "./src/AzureAppConfigurationEmulator.Data.Abstractions/AzureAppConfigurationEmulator.Data.Abstractions.csproj"
COPY "./src/AzureAppConfigurationEmulator.Data.Sqlite/AzureAppConfigurationEmulator.Data.Sqlite.csproj" "./src/AzureAppConfigurationEmulator.Data.Sqlite/AzureAppConfigurationEmulator.Data.Sqlite.csproj"
COPY "./src/AzureAppConfigurationEmulator.Keys/AzureAppConfigurationEmulator.Keys.csproj" "./src/AzureAppConfigurationEmulator.Keys/AzureAppConfigurationEmulator.Keys.csproj"
COPY "./src/AzureAppConfigurationEmulator.Labels/AzureAppConfigurationEmulator.Labels.csproj" "./src/AzureAppConfigurationEmulator.Labels/AzureAppConfigurationEmulator.Labels.csproj"
COPY "./src/AzureAppConfigurationEmulator.Locks/AzureAppConfigurationEmulator.Locks.csproj" "./src/AzureAppConfigurationEmulator.Locks/AzureAppConfigurationEmulator.Locks.csproj"
COPY "./src/AzureAppConfigurationEmulator.Messaging.EventGrid.Abstractions/AzureAppConfigurationEmulator.Messaging.EventGrid.Abstractions.csproj" "./src/AzureAppConfigurationEmulator.Messaging.EventGrid.Abstractions/AzureAppConfigurationEmulator.Messaging.EventGrid.Abstractions.csproj"
COPY "./src/AzureAppConfigurationEmulator.Messaging.EventGrid.HttpContext/AzureAppConfigurationEmulator.Messaging.EventGrid.HttpContext.csproj" "./src/AzureAppConfigurationEmulator.Messaging.EventGrid.HttpContext/AzureAppConfigurationEmulator.Messaging.EventGrid.HttpContext.csproj"
RUN dotnet restore "./src/AzureAppConfigurationEmulator/AzureAppConfigurationEmulator.csproj"
COPY . .
RUN dotnet build "./src/AzureAppConfigurationEmulator/AzureAppConfigurationEmulator.csproj" --configuration Release --output /app/build

FROM build AS publish
RUN dotnet publish "./src/AzureAppConfigurationEmulator/AzureAppConfigurationEmulator.csproj" --configuration Release --output /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AzureAppConfigurationEmulator.dll"]
