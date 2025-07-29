namespace ProcrastiN8.LazyTasks;

/// <summary>
/// Provides a default, configurable delay strategy for simulating arbitrary stalling.
/// </summary>
/// <remarks>
/// This strategy introduces a fixed or random delay, suitable for simulating indecision or network latency.
/// </remarks>
/// <remarks>
/// Initializes a new instance of the <see cref="DefaultDelayStrategy"/> class.
/// </remarks>
/// <param name="minDelay">The minimum delay duration.</param>
/// <param name="maxDelay">The maximum delay duration.</param>
/// <param name="randomFunc">An optional random function for testability.</param>
/// <param name="delayProvider">The delay provider to use for actual delays.</param>
using ProcrastiN8.JustBecause;

public class DefaultDelayStrategy(
    TimeSpan? minDelay = null,
    TimeSpan? maxDelay = null,
    IRandomProvider? randomProvider = null,
    IDelayProvider? delayProvider = null) : IDelayStrategy
{
    private readonly TimeSpan _minDelay = minDelay ?? TimeSpan.FromSeconds(1);
    private readonly TimeSpan _maxDelay = maxDelay ?? TimeSpan.FromSeconds(3);
    private readonly IRandomProvider _randomProvider = randomProvider ?? RandomProvider.Default;
    private readonly IDelayProvider _delayProvider = delayProvider ?? new TaskDelayProvider();

    /// <inheritdoc />
    public async Task DelayAsync(TimeSpan? minDelay = null, TimeSpan? maxDelay = null, Func<double, bool>? beforeCallback = null, CancellationToken cancellationToken = default)
    {
        var minMs = (int?)minDelay?.TotalMilliseconds ?? (int)_minDelay.TotalMilliseconds;
        var maxMs = (int?)maxDelay?.TotalMilliseconds ?? (int)_maxDelay.TotalMilliseconds;
        int delayMs;

        if (minMs == maxMs)
        {
            delayMs = minMs;
        }
        else
        {
            delayMs = _randomProvider.GetRandom(minMs, maxMs + 1);
            Console.WriteLine($"RandomProvider.GetRandom called with min: {minMs}, max: {maxMs + 1}, returned delay: {delayMs}");
        }

        if (beforeCallback is not null && !beforeCallback(delayMs))
        {
            return;
        }

        await _delayProvider.DelayAsync(TimeSpan.FromMilliseconds(delayMs), cancellationToken);
    }
}