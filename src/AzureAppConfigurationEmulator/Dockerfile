FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/AzureAppConfigurationEmulator/AzureAppConfigurationEmulator.csproj", "src/AzureAppConfigurationEmulator/"]
RUN dotnet restore "src/AzureAppConfigurationEmulator/AzureAppConfigurationEmulator.csproj"
COPY . .
WORKDIR "/src/src/AzureAppConfigurationEmulator"
RUN dotnet build "./AzureAppConfigurationEmulator.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./AzureAppConfigurationEmulator.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
ENV ConnectionStrings__DefaultConnection="Data Source=/var/lib/azureappconfigurationemulator/emulator.db"
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AzureAppConfigurationEmulator.dll"]
