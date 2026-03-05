# Contributing to Your Service

This template is designed to be cloned and customized for your specific service. Here's how to make it your own.

## Renaming the Service

1. **Rename the directory**:
   ```bash
   mv dotnet-service-template my-service
   cd my-service
   ```

2. **Update the namespace**:
   - Rename `src/MyService` to `src/YourServiceName`
   - Rename `tests/MyService.Tests` to `tests/YourServiceName.Tests`
   - Find/replace `MyService` with `YourServiceName` across all files

3. **Update configuration**:
   - Edit `appsettings.json` to set your service name and version
   - Update `README.md` with your service description
   - Update `CHANGELOG.md` to start fresh for your service

4. **Update Git remote**:
   ```bash
   git remote set-url origin https://github.com/yourusername/your-service.git
   ```

## Adding Dependencies

### JG Libraries

All JG libraries are commented out in `MyService.csproj` until published to NuGet. Uncomment them as they become available:

```xml
<PackageReference Include="JG.ConfigKit" Version="1.0.*" />
```

Then uncomment the corresponding registration in `Program.cs`:

```csharp
builder.AddConfigKit();
```

### External Packages

Install via `dotnet add package`:

```bash
dotnet add src/MyService package Newtonsoft.Json
```

## Code Style

- Use 4 spaces for indentation (configured in `.editorconfig`)
- Enable nullable reference types (already enabled)
- Treat warnings as errors (already enabled)
- Follow async/await patterns for all I/O
- Use `ConfigureAwait(false)` in library code
- Add XML documentation to all public APIs

## Testing

Run tests before committing:

```bash
dotnet test --configuration Release
```

Tests use:
- **xUnit** for the test framework
- **FluentAssertions** for readable assertions
- **WebApplicationFactory** for integration tests

## Building for Production

### Docker

Build the image:
```bash
docker build -t my-service:1.0.0 .
```

Run with docker-compose:
```bash
docker-compose up -d
```

### Kubernetes

Create deployment manifests in `k8s/`:
- `deployment.yaml` - Pod specification with liveness/readiness probes
- `service.yaml` - ClusterIP or LoadBalancer service
- `configmap.yaml` - Configuration overrides
- `secret.yaml` - Sensitive configuration (sealed in production)

Health endpoints for probes:
- Liveness: `/health/live`
- Readiness: `/health/ready`

## CI/CD

The GitHub Actions workflow in `.github/workflows/ci.yml` runs on every push and PR:
1. Builds on Ubuntu, Windows, and macOS
2. Runs all tests
3. Treats warnings as errors

Add deployment steps after tests pass.

## Environment Variables

Override any appsettings value via environment variables using double-underscore syntax:

```bash
export Service__Name="MyCustomService"
export Logging__Level="Debug"
export Cache__Provider="redis"
```

## Logging

Structured logging is enabled by default. All logs include:
- Correlation ID (from X-Correlation-ID header)
- Timestamp
- Log level
- Message
- Exception details (if applicable)

Logs are written to console in development, JSON in production (when JG.LoggingKit is enabled).

## Security

Before deploying to production:

1. **Rotate secrets**:
   - Replace `Auth:SecretKey` with a cryptographically random 32+ character string
   - Store in environment variables or Azure Key Vault, not in appsettings

2. **Enable HTTPS**:
   - Configure TLS certificate
   - Enforce HTTPS redirection

3. **Review rate limits**:
   - Adjust `RateLimiting:Global:PermitsPerWindow` based on expected load
   - Add per-endpoint rate limits for expensive operations

4. **Configure audit logging**:
   - Switch `Audit:Sink` from `file` to `database` for production
   - Enable hash chaining for tamper detection

## Performance

For high-throughput services:

1. **Enable Redis caching**:
   - Set `Cache:Provider` to `redis`
   - Add connection string in environment variables

2. **Tune worker pool**:
   - Increase `WorkerCount` based on CPU cores
   - Monitor queue depth and adjust `MaxQueueSize`

3. **Profile hot paths**:
   - Use BenchmarkDotNet to identify bottlenecks
   - Optimize allocation-heavy code

## Questions?

Refer to the [API documentation](docs/API.md) for endpoint and configuration details.
