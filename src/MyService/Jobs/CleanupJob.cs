namespace MyService.Jobs;

/// <summary>
/// Background job that performs periodic cleanup operations.
/// </summary>
public sealed class CleanupJob
{
    private readonly ILogger<CleanupJob> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="CleanupJob"/> class.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    public CleanupJob(ILogger<CleanupJob> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Executes the cleanup job.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    public async Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Starting cleanup job at {Timestamp}", DateTimeOffset.UtcNow);

        try
        {
            // Placeholder for real cleanup logic:
            // - Clear expired cache entries
            // - Remove stale session tokens
            // - Delete temporary files older than X days
            // - Compact audit logs
            await Task.Delay(100, cancellationToken);

            _logger.LogInformation("Cleanup job completed successfully");
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Cleanup job was cancelled");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Cleanup job failed with error: {ErrorMessage}", ex.Message);
            throw;
        }
    }
}
