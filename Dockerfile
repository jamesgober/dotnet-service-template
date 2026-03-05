# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and project files
COPY ["MyService.sln", "./"]
COPY ["src/MyService/MyService.csproj", "src/MyService/"]
COPY ["tests/MyService.Tests/MyService.Tests.csproj", "tests/MyService.Tests/"]

# Restore dependencies
RUN dotnet restore

# Copy source code
COPY . .

# Build and test
WORKDIR /src/src/MyService
RUN dotnet build -c Release --no-restore
RUN dotnet test /src/tests/MyService.Tests/MyService.Tests.csproj -c Release --no-build --no-restore

# Publish
RUN dotnet publish -c Release -o /app/publish --no-restore

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Create non-root user
RUN groupadd -r appuser && useradd -r -g appuser appuser

# Copy published application
COPY --from=build /app/publish .

# Create logs directory for audit trail
RUN mkdir -p logs && chown -R appuser:appuser logs

# Switch to non-root user
USER appuser

# Expose port
EXPOSE 8080

# Set environment
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
  CMD curl -f http://localhost:8080/status || exit 1

# Start application
ENTRYPOINT ["dotnet", "MyService.dll"]
