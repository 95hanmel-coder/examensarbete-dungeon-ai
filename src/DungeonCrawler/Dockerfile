# Steg 1: bygg spelet med .NET SDK
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY . .
RUN dotnet publish "DungeonCrawler Spel.csproj" -c Release -o /app

# Steg 2: kör spelet i en mindre bild som bara har .NET runtime
FROM mcr.microsoft.com/dotnet/runtime:10.0
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "DungeonCrawler Spel.dll"]
