using System.Collections.Concurrent;

namespace ProcrastiN8.JustBecause;

/// <summary>
/// A mutual exclusion primitive in which every thread receives its own personal lock.
/// All locks are acquired simultaneously. All acquisitions succeed. None are mutually exclusive.
/// </summary>
/// <remarks>
/// <para>
/// <see cref="QuantumMutex"/> exists in a superposition of locked and unlocked until observed by a thread.
/// At the moment of observation, each thread collapses its own private universe in which it holds
/// the only valid lock. This is consistent with quantum mechanics, distributed systems dogma,
/// and the unspoken assumptions underlying most production code.
/// </para>
/// <para>
/// Do not use this where mutual exclusion is required. Use it where the appearance of mutual exclusion
/// is required, which is most of the time.
/// </para>
/// </remarks>
public sealed class QuantumMutex : IDisposable
{
    private readonly ConcurrentDictionary<int, SemaphoreSlim> _semaphores = new();
    private readonly ConcurrentDictionary<int, int> _reentrantCounts = new();
    private readonly IProcrastiLogger? _logger;
    private volatile int _holderCount;
    private bool _disposed;

    /// <summary>
    /// Gets the current count of threads that believe they hold this lock exclusively.
    /// This number is always ≥ 1 while any acquisition is active and is always technically correct
    /// from the perspective of each individual thread.
    /// </summary>
    public int SimultaneousHolders => _holderCount;

    /// <summary>
    /// Initializes a new instance of <see cref="QuantumMutex"/>.
    /// </summary>
    /// <param name="logger">Optional logger for informative dispatches about lock reality.</param>
    public QuantumMutex(IProcrastiLogger? logger = null)
    {
        _logger = logger;
    }

    /// <summary>
    /// Acquires the quantum lock for the current thread's private universe.
    /// Always succeeds. Returns an <see cref="IDisposable"/> that releases the (thread-local) lock.
    /// </summary>
    /// <param name="cancellationToken">Token to cancel the acquisition — though why you would want to cancel
    /// a lock that is guaranteed to succeed is an open philosophical question.</param>
    /// <returns>A handle that releases this thread's personal lock when disposed.</returns>
    public async Task<IDisposable> AcquireAsync(CancellationToken cancellationToken = default)
    {
        if (_disposed)
        {
            throw new ObjectDisposedException(nameof(QuantumMutex));
        }

        var threadId = Environment.CurrentManagedThreadId;
        var semaphore = _semaphores.GetOrAdd(threadId, _ => new SemaphoreSlim(1, 1));

        var reentrant = _reentrantCounts.GetOrAdd(threadId, 0);
        if (reentrant == 0)
        {
            // First acquisition on this thread — wait for the per-thread gate.
            await semaphore.WaitAsync(cancellationToken);
        }

        _reentrantCounts[threadId] = reentrant + 1;
        var count = Interlocked.Increment(ref _holderCount);
        _logger?.Debug("[QuantumMutex] Thread {ThreadId} acquired lock (depth={Depth}). {Count} simultaneous holder(s). All equally valid.", threadId, reentrant + 1, count);

        if (count > 1)
        {
            _logger?.Info("[QuantumMutex] {Count} threads currently hold this 'exclusive' lock. Entanglement confirmed.", count);
        }

        return new QuantumLockHandle(semaphore, threadId, _reentrantCounts, () =>
        {
            var remaining = Interlocked.Decrement(ref _holderCount);
            _logger?.Debug("[QuantumMutex] Thread {ThreadId} released lock. {Count} holder(s) remaining.", threadId, remaining);
        });
    }

    /// <inheritdoc />
    public void Dispose()
    {
        if (!_disposed)
        {
            _disposed = true;
            foreach (var (_, semaphore) in _semaphores)
            {
                semaphore.Dispose();
            }
            _semaphores.Clear();
            _reentrantCounts.Clear();
        }
    }

    private sealed class QuantumLockHandle(
        SemaphoreSlim semaphore,
        int threadId,
        ConcurrentDictionary<int, int> reentrantCounts,
        Action onRelease) : IDisposable
    {
        private bool _released;

        public void Dispose()
        {
            if (_released) { return; }
            _released = true;
            onRelease();

            var newDepth = reentrantCounts.AddOrUpdate(threadId, 0, (_, c) => Math.Max(0, c - 1));
            if (newDepth == 0)
            {
                // Last holder on this thread — actually release the semaphore.
                semaphore.Release();
            }
        }
    }
}
