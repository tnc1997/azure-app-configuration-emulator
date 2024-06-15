FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY "./src/AzureAppConfigurationEmulator/AzureAppConfigurationEmulator.csproj" "./src/AzureAppConfigurationEmulator/AzureAppConfigurationEmulator.csproj"
RUN dotnet restore "./src/AzureAppConfigurationEmulator/AzureAppConfigurationEmulator.csproj"
COPY . .
RUN dotnet build "./src/AzureAppConfigurationEmulator/AzureAppConfigurationEmulator.csproj" --configuration Release --output /app/build

FROM build AS publish
RUN dotnet publish "./src/AzureAppConfigurationEmulator/AzureAppConfigurationEmulator.csproj" --configuration Release --output /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
ENV ConnectionStrings__DefaultConnection="Data Source=/var/lib/azureappconfigurationemulator/emulator.db"
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AzureAppConfigurationEmulator.dll"]
