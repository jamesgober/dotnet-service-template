using MyService.Endpoints;
using MyService.Events;
using MyService.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Configuration (JG.ConfigKit)
// Load from appsettings, environment variables, command-line args, user secrets
// Uncomment when JG.ConfigKit is published to NuGet
// builder.AddConfigKit();

// Logging (JG.LoggingKit)
// Structured logging with correlation IDs
// JSON formatter in production, console in development
// Uncomment when JG.LoggingKit is published to NuGet
// builder.AddLoggingKit();

// Error handling (JG.ErrorKit)
// Global error handler with RFC 7807 Problem Details for API errors
// Uncomment when JG.ErrorKit is published to NuGet
// builder.Services.AddErrorKit();

// Caching (JG.CacheKit)
// In-memory cache with stampede protection
// Switch to Redis in production via configuration
// Uncomment when JG.CacheKit is published to NuGet
// builder.Services.AddCacheKit(options =>
// {
//     options.DefaultTtl = TimeSpan.FromMinutes(5);
//     options.EnableStampedeProtection = true;
// });

// Rate limiting (JG.RateLimiter)
// Token bucket algorithm with per-client tracking
// Uncomment when JG.RateLimiter is published to NuGet
// builder.Services.AddRateLimiter(options =>
// {
//     options.GlobalLimit = new RateLimitConfig
//     {
//         PermitsPerWindow = 100,
//         WindowSize = TimeSpan.FromMinutes(1)
//     };
// });

// Health checks (JG.HealthKit)
// Liveness at /health/live, readiness at /health/ready
// Uncomment when JG.HealthKit is published to NuGet
// builder.Services.AddHealthKit(options =>
// {
//     options.CacheDuration = TimeSpan.FromSeconds(10);
// });

// Authentication (JG.AuthKit)
// JWT validation with configurable issuer and audience
// Uncomment when JG.AuthKit is published to NuGet
// builder.Services.AddAuthKit(options =>
// {
//     options.ValidateIssuer = true;
//     options.ValidateAudience = true;
// });

// Events (JG.EventKit)
// In-process event bus for decoupled component communication
// Uncomment when JG.EventKit is published to NuGet
// builder.Services.AddEventKit(options =>
// {
//     options.ErrorPolicy = EventErrorPolicy.LogAndContinue;
// });

// Register event handlers
builder.Services.AddSingleton<ServiceStartedEventHandler>();

// Workers (JG.WorkerKit)
// Background job queue with scheduled task support
// Uncomment when JG.WorkerKit is published to NuGet
// builder.Services.AddWorkerKit(options =>
// {
//     options.WorkerCount = 2;
//     options.MaxQueueSize = 1000;
// });

// Register background jobs
// Uncomment when JG.WorkerKit is published to NuGet
// builder.Services.AddScheduledJob<CleanupJob>("0 2 * * *"); // Daily at 2 AM

// Audit (JG.AuditKit)
// Immutable audit trail with optional hash chaining
// Uncomment when JG.AuditKit is published to NuGet
// builder.Services.AddAuditKit(options =>
// {
//     options.EnableHashChaining = true;
//     options.Sink = AuditSink.File;
// });

var app = builder.Build();

// Middleware pipeline — ORDER MATTERS
// Each middleware wraps the next, so outermost runs first

// 1. Correlation ID middleware (first — tags every request with tracking ID)
app.UseMiddleware<CorrelationIdMiddleware>();

// 2. Error handling middleware (catch all exceptions and convert to Problem Details)
// Uncomment when JG.ErrorKit is published to NuGet
// app.UseErrorKit();

// 3. Rate limiting (reject excessive requests early)
// Uncomment when JG.RateLimiter is published to NuGet
// app.UseRateLimiter();

// 4. Authentication (resolve identity from JWT)
// Uncomment when JG.AuthKit is published to NuGet
// app.UseAuthentication();

// 5. Authorization (enforce permissions)
// Uncomment when JG.AuthKit is published to NuGet
// app.UseAuthorization();

// 6. Health endpoints (no auth required, bypasses rate limits)
// Uncomment when JG.HealthKit is published to NuGet
// app.MapHealthKit();

// 7. Map API endpoints
StatusEndpoint.Map(app);

// Publish startup event
// Uncomment when JG.EventKit is published to NuGet
// var eventBus = app.Services.GetRequiredService<IEventBus>();
// await eventBus.PublishAsync(new ServiceStartedEvent
// {
//     ServiceName = "MyService",
//     Version = "1.0.0",
//     StartedAt = DateTimeOffset.UtcNow
// });

app.Run();

// Make Program accessible to tests
public partial class Program { }
