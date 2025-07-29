namespace ProcrastiN8.JustBecause.CollapseBehaviors;

public sealed class StringTheoryCollapseBehavior<T> : ICollapseBehavior<T>
{
    private readonly SemaphoreSlim _semaphore = new(11, 11);
    private readonly object _lock = new();
    private int _waitingThreads = 0;
    private TaskCompletionSource _tcs = new(TaskCreationOptions.RunContinuationsAsynchronously);

    public async Task<T?> CollapseAsync(IEnumerable<IQuantumPromise<T>> entangled, CancellationToken cancellationToken)
    {
        lock (_lock)
        {
            _waitingThreads++;
            if (_waitingThreads == 11)
            {
                _tcs.TrySetResult();
            }
        }

        var acquiredSemaphore = false;

        try
        {
            // Wait until exactly 11 threads are waiting
            await _tcs.Task.WaitAsync(cancellationToken);

            // Ensure only 11 threads proceed
            await _semaphore.WaitAsync(cancellationToken);
            acquiredSemaphore = true;

            var promises = entangled.ToArray();
            if (!promises.Any()) return default;

            return promises.First().Value;
        }
        finally
        {
            lock (_lock)
            {
                _waitingThreads--;
                if (_waitingThreads < 11)
                {
                    _tcs.TrySetResult(); // Reset for future calls
                    if (_waitingThreads == 0)
                    {
                        _tcs = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
                    }
                }
            }

            if (acquiredSemaphore)
            {
                _semaphore.Release();
            }
        }
    }
}