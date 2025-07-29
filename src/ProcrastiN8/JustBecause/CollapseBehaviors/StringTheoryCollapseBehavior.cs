namespace ProcrastiN8.JustBecause.CollapseBehaviors;

public sealed class StringTheoryCollapseBehavior<T>(IProcrastiLogger? logger = null) : ICollapseBehavior<T>
{
    private readonly SemaphoreSlim _semaphore = new(11, 11);
    private readonly object _lock = new();
    private int _waitingThreads = 0;
    private readonly TaskCompletionSource _tcs = new(TaskCreationOptions.RunContinuationsAsynchronously);
    private readonly IProcrastiLogger? _logger = logger;

    public async Task<T?> CollapseAsync(IEnumerable<IQuantumPromise<T>> entangled, CancellationToken cancellationToken)
    {
        lock (_lock)
        {
            _waitingThreads++;
            _logger?.Info($"Thread joined. Waiting threads: {_waitingThreads}");

            if (_waitingThreads == 11)
            {
                _logger?.Info("Exactly 11 threads are now waiting. Proceeding with collapse.");
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
        catch (Exception ex)
        {
            _logger?.Error("Error during thread synchronization.", ex);
            throw;
        }
        finally
        {
            if (acquiredSemaphore)
            {
                _semaphore.Release();
            }

            lock (_lock)
            {
                _waitingThreads--;
                _logger?.Info($"Thread released. Remaining threads: {_waitingThreads}");
            }
        }
    }
}