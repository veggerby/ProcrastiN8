using ProcrastiN8.Metrics;

namespace ProcrastiN8.Services;

public class RetryService
{
    // Increment value for retry metric
    private const int RetryIncrement = 1;

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
                await Task.Delay(500, cancellationToken);
            }
        }

        throw new InvalidOperationException("Max retries exceeded.");
    }
}
