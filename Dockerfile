# 1. Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Kopiraj sve fajlove
COPY . .

# Restore NuGet paketa za glavni API projekat
RUN dotnet restore "HomeExchange-API/HomeExchange.API.csproj"

# Build i publish u Release konfiguraciji
RUN dotnet publish "HomeExchange-API/HomeExchange.API.csproj" -c Release -o /app/publish

# 2. Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Kopiraj build iz prethodnog stage-a
COPY --from=build /app/publish .

# Railway port
ENV ASPNETCORE_URLS=http://+:$PORT
EXPOSE $PORT

# Start aplikacije
ENTRYPOINT ["dotnet", "HomeExchange.API.dll"]
