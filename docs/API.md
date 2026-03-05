# API Reference

## Endpoints

### GET /status

Returns service status and runtime information.

**Response** (200 OK):
```json
{
  "serviceName": "MyService",
  "version": "1.0.0",
  "environment": "Development",
  "status": "Running",
  "uptime": "5m 23s",
  "timestamp": "2026-03-04T12:34:56.789Z"
}
```

**Fields**:
- `serviceName` (string): The name of the service
- `version` (string): The semantic version
- `environment` (string): Current deployment environment (Development, Staging, Production)
- `status` (string): Service status, always "Running" if the endpoint responds
- `uptime` (string): Human-readable uptime (e.g., "2d 5h 12m", "45m 3s")
- `timestamp` (DateTimeOffset): UTC timestamp of the response

**Example**:
```bash
curl http://localhost:5000/status
```

---

## Middleware

### Correlation ID Middleware

Automatically adds `X-Correlation-ID` header to every request and response.

- If the client provides `X-Correlation-ID` in the request, it is preserved
- If no header is present, a new GUID is generated
- The correlation ID is available in `HttpContext.Items["CorrelationId"]`
- All logs for the request include the correlation ID in the scope

**Example Request**:
```http
GET /status HTTP/1.1
Host: localhost:5000
X-Correlation-ID: abc123def456
```

**Example Response**:
```http
HTTP/1.1 200 OK
X-Correlation-ID: abc123def456
Content-Type: application/json
```

---

## Configuration

All configuration is read from `appsettings.json` and environment-specific overrides.

### Service Configuration

```json
{
  "Service": {
    "Name": "MyService",
    "Version": "1.0.0"
  }
}
```

### Logging Configuration

```json
{
  "Logging": {
    "Level": "Information",
    "Format": "console",
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

### Cache Configuration

```json
{
  "Cache": {
    "DefaultTtlSeconds": 300,
    "Provider": "memory"
  }
}
```

**Providers**: `memory`, `redis` (requires JG.CacheKit.Redis)

### Rate Limiting Configuration

```json
{
  "RateLimiting": {
    "Global": {
      "PermitsPerWindow": 100,
      "WindowSeconds": 60
    }
  }
}
```

### Auth Configuration

```json
{
  "Auth": {
    "Issuer": "https://localhost",
    "Audience": "myservice",
    "SecretKey": "CHANGE-THIS-IN-PRODUCTION-minimum-32-chars!"
  }
}
```

**Security Note**: Never commit real secret keys. Use user secrets for development and environment variables in production.

### Health Check Configuration

```json
{
  "HealthChecks": {
    "CacheDurationSeconds": 10
  }
}
```

### Audit Configuration

```json
{
  "Audit": {
    "Enabled": true,
    "HashChaining": true,
    "Sink": "file",
    "FilePath": "logs/audit.jsonl"
  }
}
```

**Sinks**: `file`, `database` (requires JG.AuditKit.Database)

---

## Adding Your Own Endpoints

Create a new file in `Endpoints/`:

```csharp
namespace MyService.Endpoints;

public static class GreetingEndpoint
{
    public static void Map(WebApplication app)
    {
        app.MapGet("/greet/{name}", (string name) => 
            Results.Ok(new { Greeting = $"Hello, {name}!" }));
    }
}
```

Register it in `Program.cs`:

```csharp
// Map API endpoints
StatusEndpoint.Map(app);
GreetingEndpoint.Map(app);
```

---

## Background Jobs

Background jobs run via JG.WorkerKit. Create a job class:

```csharp
namespace MyService.Jobs;

public sealed class MyJob
{
    private readonly ILogger<MyJob> _logger;

    public MyJob(ILogger<MyJob> logger)
    {
        _logger = logger;
    }

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        // Job logic here
    }
}
```

Register it as a scheduled job in `Program.cs`:

```csharp
builder.Services.AddScheduledJob<MyJob>("0 */5 * * *"); // Every 5 minutes
```

---

## Events

Define an event and handler:

```csharp
namespace MyService.Events;

public sealed class OrderPlacedEvent
{
    public required string OrderId { get; init; }
    public required decimal Total { get; init; }
}

public sealed class OrderPlacedHandler
{
    private readonly ILogger<OrderPlacedHandler> _logger;

    public OrderPlacedHandler(ILogger<OrderPlacedHandler> logger)
    {
        _logger = logger;
    }

    public Task HandleAsync(OrderPlacedEvent @event, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Order {OrderId} placed for {Total}", @event.OrderId, @event.Total);
        return Task.CompletedTask;
    }
}
```

Register the handler in `Program.cs`:

```csharp
builder.Services.AddSingleton<OrderPlacedHandler>();
```

Publish events from your endpoint:

```csharp
var eventBus = context.RequestServices.GetRequiredService<IEventBus>();
await eventBus.PublishAsync(new OrderPlacedEvent 
{ 
    OrderId = "12345", 
    Total = 99.99m 
});
