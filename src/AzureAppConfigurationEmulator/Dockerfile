FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY "./src/AzureAppConfigurationEmulator/AzureAppConfigurationEmulator.csproj" "./src/AzureAppConfigurationEmulator/AzureAppConfigurationEmulator.csproj"
RUN dotnet restore "./src/AzureAppConfigurationEmulator/AzureAppConfigurationEmulator.csproj"
COPY . .
RUN dotnet build "./src/AzureAppConfigurationEmulator/AzureAppConfigurationEmulator.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "./src/AzureAppConfigurationEmulator/AzureAppConfigurationEmulator.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

RUN apt-get update && apt-get -y install openssh-client

COPY --from=publish /app/publish .
COPY "keygen.sh" .
COPY "container-entrypoint.sh" .

RUN chmod +x keygen.sh && chmod +x container-entrypoint.sh

ENTRYPOINT ["./container-entrypoint.sh"]
