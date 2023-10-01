FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY "./src/AzureAppConfigurationEmulator/AzureAppConfigurationEmulator.csproj" "./src/AzureAppConfigurationEmulator"
RUN dotnet restore "./src/AzureAppConfigurationEmulator/AzureAppConfigurationEmulator.csproj"
COPY . .
RUN dotnet build "./src/AzureAppConfigurationEmulator/AzureAppConfigurationEmulator.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "./src/AzureAppConfigurationEmulator/AzureAppConfigurationEmulator.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AzureAppConfigurationEmulator.dll"]
