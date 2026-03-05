# dotnet-service-template

[![License](https://img.shields.io/badge/license-Apache--2.0-blue.svg)](./LICENSE)
[![CI](https://github.com/jamesgober/dotnet-service-template/actions/workflows/ci.yml/badge.svg)](https://github.com/jamesgober/dotnet-service-template/actions)

A production-ready .NET 8 service template with the complete JG .NET Library Collection pre-wired. Clone it, rename it, and start building your service. All the infrastructure code is already done.

## What's Inside

Every JG library is integrated with sensible defaults and real examples:

- **JG.ConfigKit** — Multi-source configuration (appsettings, env vars, CLI args, user secrets)
- **JG.ErrorKit** — Typed errors with RFC 7807 Problem Details for APIs
- **JG.LoggingKit** — Structured logging with correlation IDs and JSON formatting
- **JG.CacheKit** — In-memory and distributed caching with stampede protection
- **JG.RateLimiter** — Token bucket and sliding window rate limiting
- **JG.HealthKit** — Kubernetes-ready liveness and readiness endpoints
- **JG.AuthKit** — JWT authentication with refresh tokens and RBAC
- **JG.EventKit** — In-process event bus for decoupled components
- **JG.WorkerKit** — Background job queue with cron scheduling
- **JG.AuditKit** — Immutable audit logging with optional hash chaining

Plus working examples: status endpoint, correlation ID middleware, startup event, scheduled cleanup job.

## Quick Start

```bash
git clone https://github.com/jamesgober/dotnet-service-template.git my-service
cd my-service
dotnet run --project src/MyService
```

The service starts on `http://localhost:5000`. Test it:

```bash
curl http://localhost:5000/status
```

## What's Included

### Endpoints
- **GET /status** — Service info, version, uptime, environment

### Middleware
- **CorrelationIdMiddleware** — Tracks every request with `X-Correlation-ID`

### Events
- **ServiceStartedEvent** — Published when the host starts, demonstrates event bus pattern

### Background Jobs
- **CleanupJob** — Placeholder for scheduled maintenance tasks

### Configuration
Working appsettings for development and production environments. Everything has sensible defaults that work out of the box.

## Middleware Pipeline Order

The order matters. Each middleware wraps the next:

1. **Correlation ID** — Tag every request first
2. **Error handling** — Catch exceptions and convert to Problem Details
3. **Rate limiting** — Reject excessive traffic early
4. **Authentication** — Resolve JWT identity
5. **Authorization** — Enforce permissions
6. **Health endpoints** — Bypass auth for /health/live and /health/ready
7. **Application endpoints** — Your API

This order ensures requests are tracked, failures are handled gracefully, and auth happens before business logic.

## Adding Your Own Code

### New Endpoint
Create a file in `src/MyService/Endpoints/`:

```csharp
public static class GreetingEndpoint
{
    public static void Map(WebApplication app)
    {
        app.MapGet("/greet/{name}", (string name) => 
            Results.Ok(new { Greeting = $"Hello, {name}!" }));
    }
}
```

Register in `Program.cs`:
```csharp
GreetingEndpoint.Map(app);
```

### New Event
Create event and handler in `src/MyService/Events/`:

```csharp
public sealed class OrderPlacedEvent
{
    public required string OrderId { get; init; }
}

public sealed class OrderPlacedHandler
{
    private readonly ILogger<OrderPlacedHandler> _logger;

    public OrderPlacedHandler(ILogger<OrderPlacedHandler> logger) => _logger = logger;

    public Task HandleAsync(OrderPlacedEvent @event, CancellationToken ct)
    {
        _logger.LogInformation("Order {OrderId} placed", @event.OrderId);
        return Task.CompletedTask;
    }
}
```

Register in `Program.cs`:
```csharp
builder.Services.AddSingleton<OrderPlacedHandler>();
```

Publish from anywhere:
```csharp
await eventBus.PublishAsync(new OrderPlacedEvent { OrderId = "12345" });
```

### New Background Job
Create a job in `src/MyService/Jobs/`:

```csharp
public sealed class ReportGenerationJob
{
    public async Task ExecuteAsync(CancellationToken ct)
    {
        // Generate reports
    }
}
```

Schedule in `Program.cs`:
```csharp
builder.Services.AddScheduledJob<ReportGenerationJob>("0 2 * * *"); // Daily at 2 AM
```

## Configuration

All settings in `appsettings.json`. Override per environment with `appsettings.{Environment}.json`.

**Service info**:
```json
"Service": {
  "Name": "MyService",
  "Version": "1.0.0"
}
```

**Logging**:
```json
"Logging": {
  "Level": "Information",
  "Format": "console"
}
```

**Cache**:
```json
"Cache": {
  "DefaultTtlSeconds": 300,
  "Provider": "memory"
}
```

Switch to Redis in production: `"Provider": "redis"` (requires JG.CacheKit.Redis package).

**Rate limiting**:
```json
"RateLimiting": {
  "Global": {
    "PermitsPerWindow": 100,
    "WindowSeconds": 60
  }
}
```

**Auth**:
```json
"Auth": {
  "Issuer": "https://localhost",
  "Audience": "myservice",
  "SecretKey": "CHANGE-THIS-IN-PRODUCTION-minimum-32-chars!"
}
```

Never commit real secrets. Use user secrets for dev, environment variables for prod.

**Audit**:
```json
"Audit": {
  "Enabled": true,
  "HashChaining": true,
  "Sink": "file",
  "FilePath": "logs/audit.jsonl"
}
```

## Building and Testing

```bash
# Restore dependencies
dotnet restore

# Build
dotnet build --configuration Release

# Run tests
dotnet test --configuration Release

# Run the service
dotnet run --project src/MyService
```

All JG packages are referenced as NuGet dependencies. If a package isn't published yet, its registration is commented out in `Program.cs`. Uncomment when available.

## Project Structure

```
src/
├── MyService/
│   ├── Program.cs                    # Host builder, DI, middleware pipeline
│   ├── appsettings.json              # Base configuration
│   ├── appsettings.Development.json  # Dev overrides
│   ├── Endpoints/                    # API endpoint handlers
│   ├── Events/                       # Event definitions and handlers
│   ├── Jobs/                         # Background job implementations
│   └── Middleware/                   # Custom middleware
tests/
├── MyService.Tests/                  # xUnit tests with WebApplicationFactory
docs/
├── API.md                            # Endpoint and configuration docs
```

## Documentation

- [API Reference](docs/API.md) — Endpoints, middleware, configuration
- [Changelog](CHANGELOG.md) — Version history

## License

Apache 2.0 — see [LICENSE](LICENSE)

## Author

James Gober — [github.com/jamesgober](https://github.com/jamesgober)
