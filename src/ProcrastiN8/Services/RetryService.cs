using ProcrastiN8.Metrics;

namespace ProcrastiN8.Services;

public class RetryService
{
    public async Task<T> RetryUntilDone<T>(Func<Task<T>> action, int maxAttempts)
    {
        for (int attempt = 1; attempt <= maxAttempts; attempt++)
        {
            try
            {
                return await action();
            }
            catch
            {
                ProcrastinationMetrics.RetryAttempts.Add(1);
                await Task.Delay(500);
            }
        }

        throw new InvalidOperationException("Max retries exceeded.");
    }
}