# dotnet-service-template

[![License](https://img.shields.io/badge/license-Apache--2.0-blue.svg)](./LICENSE)
[![CI](https://github.com/jamesgober/dotnet-service-template/actions/workflows/ci.yml/badge.svg)](https://github.com/jamesgober/dotnet-service-template/actions)

---

A production-ready project template for .NET 8 services. Pre-wired with the complete JG .NET Library Collection — configuration, logging, error handling, caching, rate limiting, health checks, authentication, events, background workers, and audit logging. Clone it, rename it, start building.

## What's Included

- **JG.ConfigKit** — Multi-source configuration with hot-reload
- **JG.ErrorKit** — Typed errors with RFC 7807 Problem Details
- **JG.LoggingKit** — Structured logging with correlation IDs
- **JG.CacheKit** — Memory + distributed caching with stampede protection
- **JG.RateLimiter** — Token bucket, sliding window, per-client rate limiting
- **JG.HealthKit** — Kubernetes-ready liveness and readiness probes
- **JG.AuthKit** — JWT authentication, refresh tokens, RBAC
- **JG.EventKit** — In-process event bus for decoupled components
- **JG.WorkerKit** — Background jobs and scheduled tasks
- **JG.AuditKit** — Immutable audit trail logging
## Quick Start

```bash
# Clone the template
git clone https://github.com/jamesgober/dotnet-service-template.git my-service
cd my-service

# Restore and run
dotnet restore
dotnet run
```

## Project Structure

```
src/
├── MyService/
│   ├── Program.cs              # Application entry point with middleware pipeline
│   ├── appsettings.json        # Base configuration
│   ├── appsettings.Development.json
│   └── Endpoints/              # API endpoint handlers
tests/
├── MyService.Tests/
docs/
├── API.md
```

## Documentation

- **[API Reference](./docs/API.md)** — Full API documentation and examples

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## License

Licensed under the Apache License 2.0. See [LICENSE](./LICENSE) for details.

---

**Ready to get started?** Clone the repo and start building your service.