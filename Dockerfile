# Stage 1 — build stage (SDK image, larger)
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy solution and project files first (layer caching)
COPY ["TaskManager.slnx", "."]
COPY ["TaskManager.API/TaskManager.API.csproj",                         "TaskManager.API/"]
COPY ["TaskManager.Application/TaskManager.Application.csproj",         "TaskManager.Application/"]
COPY ["TaskManager.Infrastructure/TaskManager.Infrastructure.csproj",   "TaskManager.Infrastructure/"]
COPY ["TaskManager.Domain/TaskManager.Domain.csproj",                   "TaskManager.Domain/"]
COPY ["TaskManager.Tests/TaskManager.Tests.csproj",                     "TaskManager.Tests/"]

# Restore — cached unless .csproj files change
RUN dotnet restore

# Copy everything else and build
COPY . .

# Run tests inside Docker before publishing
RUN dotnet test --configuration Release --no-restore

RUN dotnet publish TaskManager.API/TaskManager.API.csproj \
    --configuration Release \
    --output /app/publish \
    --no-restore

# Stage 2 — runtime stage (runtime-only image, much smaller)
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "TaskManager.API.dll"]