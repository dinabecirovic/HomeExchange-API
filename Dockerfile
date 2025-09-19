# 1. Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Kopiraj .sln i sve projekte
COPY HomeExchange.sln .
COPY HomeExchange/HomeExchange.API.csproj HomeExchange/
COPY HomeExchange.Data/*.csproj HomeExchange.Data/
COPY HomeExchange.Interfaces/*.csproj HomeExchange.Interfaces/
COPY HomeExchange.Services/*.csproj HomeExchange.Services/

# Restore NuGet paketa
RUN dotnet restore HomeExchange.sln

# Kopiraj ceo kod
COPY . .

# Build i publish glavnog API projekta
WORKDIR /src/HomeExchange
RUN dotnet publish HomeExchange.API.csproj -c Release -o /app/publish

# 2. Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

# Railway port
ENV ASPNETCORE_URLS=http://+:$PORT
EXPOSE $PORT

ENTRYPOINT ["dotnet", "HomeExchange.API.dll"]
