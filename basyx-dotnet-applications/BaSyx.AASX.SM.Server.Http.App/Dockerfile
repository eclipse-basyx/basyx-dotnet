# -------- Build Stage --------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy all data to image
COPY . .

RUN dotnet restore BaSyx.AASX.SM.Server.Http.App.csproj
# Rebuild project (including all dependencies)
RUN dotnet build BaSyx.AASX.SM.Server.Http.App.csproj -c Release --no-restore --no-incremental

RUN dotnet publish BaSyx.AASX.SM.Server.Http.App.csproj -c Release -o /app/publish \
    --no-restore --no-build /p:PublishReadyToRun=false

# -------- Runtime Stage --------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "BaSyx.AASX.SM.Server.Http.App.dll"]
