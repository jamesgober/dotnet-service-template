namespace MyService.Middleware;

/// <summary>
/// Middleware that ensures every request has a correlation ID for distributed tracing.
/// </summary>
public sealed class CorrelationIdMiddleware
{
    private const string CorrelationIdHeader = "X-Correlation-ID";
    private const string CorrelationIdKey = "CorrelationId";

    private readonly RequestDelegate _next;
    private readonly ILogger<CorrelationIdMiddleware> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="CorrelationIdMiddleware"/> class.
    /// </summary>
    /// <param name="next">The next middleware in the pipeline.</param>
    /// <param name="logger">The logger instance.</param>
    public CorrelationIdMiddleware(RequestDelegate next, ILogger<CorrelationIdMiddleware> logger)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Invokes the middleware.
    /// </summary>
    /// <param name="context">The HTTP context.</param>
    public async Task InvokeAsync(HttpContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        string correlationId;

        if (context.Request.Headers.TryGetValue(CorrelationIdHeader, out var headerValue) &&
            !string.IsNullOrWhiteSpace(headerValue))
        {
            correlationId = headerValue.ToString();
            _logger.LogDebug("Using existing correlation ID: {CorrelationId}", correlationId);
        }
        else
        {
            correlationId = Guid.NewGuid().ToString("N");
            _logger.LogDebug("Generated new correlation ID: {CorrelationId}", correlationId);
        }

        context.Items[CorrelationIdKey] = correlationId;
        context.Response.Headers[CorrelationIdHeader] = correlationId;

        using (_logger.BeginScope(new Dictionary<string, object>
        {
            [CorrelationIdKey] = correlationId
        }))
        {
            await _next(context).ConfigureAwait(false);
        }
    }
}
