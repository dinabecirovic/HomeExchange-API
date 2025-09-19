# 1. Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Kopiraj sln fajl
COPY *.sln .

# Kopiraj csproj fajlove iz HomeExchange foldera
COPY HomeExchange/HomeExchange.Data/*.csproj ./HomeExchange.Data/
COPY HomeExchange/HomeExchange.Interfaces/*.csproj ./HomeExchange.Interfaces/
COPY HomeExchange/HomeExchange.Services/*.csproj ./HomeExchange.Services/
COPY HomeExchange/HomeExchange.API/*.csproj ./HomeExchange.API/

# Restore pakete
RUN dotnet restore

# Kopiraj ceo kod
COPY HomeExchange/. ./HomeExchange/
WORKDIR /app/HomeExchange/HomeExchange.API
RUN dotnet publish -c Release -o /app/publish

# 2. Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:$PORT
EXPOSE $PORT

ENTRYPOINT ["dotnet", "HomeExchange.API.dll"]
