namespace MyService.Endpoints;

/// <summary>
/// Status endpoint that returns service information and health status.
/// </summary>
public static class StatusEndpoint
{
    private static readonly DateTimeOffset StartupTime = DateTimeOffset.UtcNow;

    /// <summary>
    /// Maps the status endpoint to the application.
    /// </summary>
    /// <param name="app">The web application.</param>
    public static void Map(WebApplication app)
    {
        app.MapGet("/status", GetStatus)
            .WithName("GetStatus")
            .Produces<StatusResponse>(StatusCodes.Status200OK);
    }

    private static IResult GetStatus(HttpContext context, IConfiguration configuration)
    {
        var serviceName = configuration["Service:Name"] ?? "MyService";
        var version = configuration["Service:Version"] ?? "1.0.0";
        var environment = context.RequestServices.GetRequiredService<IHostEnvironment>().EnvironmentName;
        var uptime = DateTimeOffset.UtcNow - StartupTime;

        var response = new StatusResponse
        {
            ServiceName = serviceName,
            Version = version,
            Environment = environment,
            Status = "Running",
            Uptime = FormatUptime(uptime),
            Timestamp = DateTimeOffset.UtcNow
        };

        return Results.Ok(response);
    }

    private static string FormatUptime(TimeSpan uptime)
    {
        if (uptime.TotalDays >= 1)
            return $"{(int)uptime.TotalDays}d {uptime.Hours}h {uptime.Minutes}m";
        if (uptime.TotalHours >= 1)
            return $"{(int)uptime.TotalHours}h {uptime.Minutes}m";
        if (uptime.TotalMinutes >= 1)
            return $"{(int)uptime.TotalMinutes}m {uptime.Seconds}s";
        return $"{uptime.Seconds}s";
    }
}

/// <summary>
/// Response model for the status endpoint.
/// </summary>
public sealed class StatusResponse
{
    /// <summary>
    /// Gets or sets the service name.
    /// </summary>
    public required string ServiceName { get; init; }

    /// <summary>
    /// Gets or sets the service version.
    /// </summary>
    public required string Version { get; init; }

    /// <summary>
    /// Gets or sets the deployment environment.
    /// </summary>
    public required string Environment { get; init; }

    /// <summary>
    /// Gets or sets the current status.
    /// </summary>
    public required string Status { get; init; }

    /// <summary>
    /// Gets or sets the formatted uptime string.
    /// </summary>
    public required string Uptime { get; init; }

    /// <summary>
    /// Gets or sets the response timestamp.
    /// </summary>
    public required DateTimeOffset Timestamp { get; init; }
}
