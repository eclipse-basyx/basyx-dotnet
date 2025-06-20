# -------- Build Stage --------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Projektdateien ins Image kopieren
COPY . .

# Wiederherstellen aller NuGet-Abhängigkeiten
RUN dotnet restore BaSyx.AASX.SM.Server.Http.App.csproj

# Komplett neuer Build (inkl. aller Projektreferenzen)
RUN dotnet build BaSyx.AASX.SM.Server.Http.App.csproj -c Release --no-restore --no-incremental

# Veröffentlichen des App-Projekts (ohne erneut zu bauen)
RUN dotnet publish BaSyx.AASX.SM.Server.Http.App.csproj -c Release -o /app/publish \
    --no-restore --no-build /p:PublishReadyToRun=false

# -------- Runtime Stage --------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Veröffentlichte Dateien aus Build-Stufe übernehmen
COPY --from=build /app/publish .

# Start der Anwendung
ENTRYPOINT ["dotnet", "BaSyx.AASX.SM.Server.Http.App.dll"]
