# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.0.0] - 2026-03-04

### Added
- Initial service template with complete JG .NET Library Collection integration
- Example status endpoint at GET /status with service info and uptime
- Correlation ID middleware for distributed tracing
- ServiceStartedEvent and handler demonstrating event bus pattern
- CleanupJob placeholder for scheduled background tasks
- Development and production configuration profiles with sensible defaults
- WebApplicationFactory-based integration tests for endpoints and middleware
- Comprehensive API documentation with examples
- Complete middleware pipeline setup with proper ordering
- Support for all 10 JG libraries (ConfigKit, ErrorKit, LoggingKit, CacheKit, RateLimiter, HealthKit, AuthKit, EventKit, WorkerKit, AuditKit)

[1.0.0]: https://github.com/jamesgober/dotnet-service-template/releases/tag/v1.0.0
