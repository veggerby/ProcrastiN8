using ProcrastiN8.LazyTasks;
using ProcrastiN8.Metrics;

namespace ProcrastiN8.Services;

/// <summary>
/// Retries a task until it succeeds, runs out of attempts, or gives up for reasons it declines to elaborate on.
/// </summary>
public class RetryService(IDelayProvider? delayProvider = null)
{
    // Increment value for retry metric
    private const int RetryIncrement = 1;
    // Default backoff delay between retry attempts
    private static readonly TimeSpan RetryDelayInterval = TimeSpan.FromMilliseconds(500);

    private readonly IDelayProvider _delayProvider = delayProvider ?? new TaskDelayProvider();

    /// <summary>
    /// Retries the given asynchronous operation until it returns a result, the cancellation token fires,
    /// or the maximum number of attempts is exhausted — whichever comes first.
    /// </summary>
    public async Task<T> RetryUntilDone<T>(Func<Task<T>> action, int maxAttempts, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(action);
        if (maxAttempts <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(maxAttempts), "Max attempts must be greater than zero.");
        }

        for (int attempt = 1; attempt <= maxAttempts; attempt++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            try
            {
                return await action();
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch
            {
                ProcrastinationMetrics.RetryAttempts.Add(RetryIncrement);
                await _delayProvider.DelayAsync(RetryDelayInterval, cancellationToken);
            }
        }

        throw new InvalidOperationException("Max retries exceeded.");
    }
}
