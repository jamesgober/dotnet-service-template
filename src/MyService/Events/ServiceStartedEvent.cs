namespace MyService.Events;

/// <summary>
/// Event published when the service starts.
/// </summary>
public sealed class ServiceStartedEvent
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
    /// Gets or sets the timestamp when the service started.
    /// </summary>
    public required DateTimeOffset StartedAt { get; init; }
}

/// <summary>
/// Handler for ServiceStartedEvent.
/// </summary>
public sealed class ServiceStartedEventHandler
{
    private readonly ILogger<ServiceStartedEventHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceStartedEventHandler"/> class.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    public ServiceStartedEventHandler(ILogger<ServiceStartedEventHandler> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Handles the ServiceStartedEvent.
    /// </summary>
    /// <param name="event">The event to handle.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public Task HandleAsync(ServiceStartedEvent @event, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(@event);

        _logger.LogInformation(
            "Service {ServiceName} v{Version} started at {StartedAt}",
            @event.ServiceName,
            @event.Version,
            @event.StartedAt);

        return Task.CompletedTask;
    }
}
