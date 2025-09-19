# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Kopiraj sve fajlove
COPY . .

# Restore nuget paketa
RUN dotnet restore "HomeExchange-API/HomeExchange-API.csproj"

# Build i publish
RUN dotnet publish "HomeExchange-API/HomeExchange-API.csproj" -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Kopiraj build iz build stage-a
COPY --from=build /app/publish .

# Railway koristi port iz env varijable $PORT
ENV ASPNETCORE_URLS=http://*:$PORT
EXPOSE $PORT

ENTRYPOINT ["dotnet", "HomeExchange-API.dll"]
