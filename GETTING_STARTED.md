# Getting Started

This guide walks you through cloning, customizing, and running your first service based on this template.

## Prerequisites

- .NET 8 SDK or later
- Git
- (Optional) Docker for containerized deployment

## Clone and Run

```bash
# Clone the template
git clone https://github.com/jamesgober/dotnet-service-template.git my-new-service
cd my-new-service

# Run the service
dotnet run --project src/MyService

# In another terminal, test it
curl http://localhost:5000/status
```

Expected response:
```json
{
  "serviceName": "MyService",
  "version": "1.0.0",
  "environment": "Development",
  "status": "Running",
  "uptime": "5s",
  "timestamp": "2026-03-04T12:34:56.789Z"
}
```

## Customize the Service

### 1. Rename Everything

**Option A: Manual**
- Rename directories: `src/MyService` → `src/YourService`
- Find/replace `MyService` with `YourService` in all files
- Update namespaces in all `.cs` files

**Option B: Script** (PowerShell)
```powershell
$oldName = "MyService"
$newName = "YourService"

# Rename directories
Rename-Item "src\$oldName" "src\$newName"
Rename-Item "tests\${oldName}.Tests" "tests\${newName}.Tests"

# Update file contents
Get-ChildItem -Recurse -Include *.cs,*.csproj,*.json,*.md |
  ForEach-Object {
    (Get-Content $_.FullName) -replace $oldName, $newName |
      Set-Content $_.FullName
  }
```

### 2. Update Configuration

Edit `appsettings.json`:
```json
{
  "Service": {
    "Name": "YourService",
    "Version": "1.0.0"
  }
}
```

### 3. Add Your First Endpoint

Create `src/YourService/Endpoints/GreetingEndpoint.cs`:
```csharp
namespace YourService.Endpoints;

public static class GreetingEndpoint
{
    public static void Map(WebApplication app)
    {
        app.MapGet("/greet/{name}", (string name) =>
        {
            return Results.Ok(new { Greeting = $"Hello, {name}!" });
        })
        .WithName("Greet")
        .Produces<object>(StatusCodes.Status200OK);
    }
}
```

Register in `Program.cs`:
```csharp
// Map API endpoints
StatusEndpoint.Map(app);
GreetingEndpoint.Map(app); // Add this line
```

Test it:
```bash
curl http://localhost:5000/greet/World
```

### 4. Add Your First Background Job

Create `src/YourService/Jobs/DailyReportJob.cs`:
```csharp
namespace YourService.Jobs;

public sealed class DailyReportJob
{
    private readonly ILogger<DailyReportJob> _logger;

    public DailyReportJob(ILogger<DailyReportJob> logger)
    {
        _logger = logger;
    }

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Generating daily report");

        // Your report generation logic here

        _logger.LogInformation("Daily report completed");
    }
}
```

Schedule in `Program.cs`:
```csharp
// Uncomment when JG.WorkerKit is available
// builder.Services.AddScheduledJob<DailyReportJob>("0 2 * * *");
```

## Enable JG Libraries

As JG libraries are published to NuGet, uncomment them in `MyService.csproj`:

```xml
<PackageReference Include="JG.ConfigKit" Version="1.0.*" />
<PackageReference Include="JG.ErrorKit" Version="1.0.*" />
<!-- etc -->
```

Then uncomment the registrations in `Program.cs`:

```csharp
builder.AddConfigKit();
builder.Services.AddErrorKit();
// etc
```

## Run Tests

```bash
dotnet test --configuration Release
```

All 10 tests should pass.

## Docker Deployment

Build and run:
```bash
docker-compose up --build
```

The service runs on http://localhost:5000 with health checks enabled.

## Production Checklist

Before deploying to production:

- [ ] Rotate the `Auth:SecretKey` in appsettings.json
- [ ] Store secrets in environment variables or key vault
- [ ] Enable HTTPS with valid TLS certificate
- [ ] Review rate limits for your expected load
- [ ] Switch cache provider to Redis for multi-instance deployments
- [ ] Configure audit sink to database instead of file
- [ ] Set `ASPNETCORE_ENVIRONMENT=Production`
- [ ] Configure health check endpoints for orchestrator probes
- [ ] Set up structured logging to your log aggregation service
- [ ] Enable application monitoring (Application Insights, etc.)

## Next Steps

- Read [API.md](docs/API.md) for detailed endpoint documentation
- Read [CONTRIBUTING.md](CONTRIBUTING.md) for development guidelines
- Check the [JG .NET Library Collection](https://github.com/jamesgober) for library documentation

## Troubleshooting

**Build errors about missing packages?**
- The JG libraries are commented out until published to NuGet
- The template builds and runs without them

**Port already in use?**
- Change `applicationUrl` in `Properties/launchSettings.json`
- Or set environment variable: `export ASPNETCORE_URLS="http://localhost:5555"`

**Tests failing?**
- Ensure you're running from the repository root
- Run `dotnet restore` before `dotnet test`
- Check that no other instance of the service is running

## Support

For issues with the template: https://github.com/jamesgober/dotnet-service-template/issues

For JG library questions: Check individual library repositories
