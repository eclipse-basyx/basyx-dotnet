#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /app

COPY *.csproj ./
RUN dotnet restore BaSyx.Registry.Server.Http.App.csproj --disable-parallel
COPY . ./
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/sdk:6.0
WORKDIR /app
COPY . ./
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "BaSyx.Registry.Server.Http.App.dll"]