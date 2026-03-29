# Stage 1: restore (cached layer — only re-runs when .csproj changes)
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY BlazorPortfolio/BlazorPortfolio.csproj BlazorPortfolio/
RUN dotnet restore BlazorPortfolio/BlazorPortfolio.csproj

# Stage 2: copy source and publish
COPY BlazorPortfolio/ BlazorPortfolio/
RUN dotnet publish BlazorPortfolio/BlazorPortfolio.csproj \
    -c Release \
    --no-restore \
    -o /app/publish

# Stage 3: runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

# Render injects $PORT at runtime; fall back to 8080 locally
ENTRYPOINT ASPNETCORE_URLS=http://+:${PORT:-8080} dotnet BlazorPortfolio.dll
