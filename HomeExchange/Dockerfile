# 1. Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Kopiraj csproj fajlove i restore pakete
COPY *.sln .
COPY HomeExchange.Data/*.csproj ./HomeExchange.Data/
COPY HomeExchange.Interfaces/*.csproj ./HomeExchange.Interfaces/
COPY HomeExchange.Services/*.csproj ./HomeExchange.Services/
COPY HomeExchange.API/*.csproj ./HomeExchange.API/

RUN dotnet restore

# Kopiraj ceo kod i build
COPY . .
WORKDIR /app/HomeExchange.API
RUN dotnet publish -c Release -o /app/publish

# 2. Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

# Railway automatski dodeljuje port preko $PORT
ENV ASPNETCORE_URLS=http://+:$PORT
EXPOSE $PORT

ENTRYPOINT ["dotnet", "HomeExchange.API.dll"]
