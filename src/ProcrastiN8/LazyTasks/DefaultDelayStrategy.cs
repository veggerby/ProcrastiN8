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
public class DefaultDelayStrategy(
    TimeSpan? minDelay = null,
    TimeSpan? maxDelay = null,
    Func<int, int>? randomFunc = null,
    IDelayProvider? delayProvider = null) : IDelayStrategy
{
    private readonly TimeSpan _minDelay = minDelay ?? TimeSpan.FromSeconds(1);
    private readonly TimeSpan _maxDelay = maxDelay ?? TimeSpan.FromSeconds(3);
    private readonly Func<int, int>? _randomFunc = randomFunc;
    private readonly IDelayProvider _delayProvider = delayProvider ?? new TaskDelayProvider();

    /// <inheritdoc />
    public async Task DelayAsync(CancellationToken cancellationToken = default)
    {
        var minMs = (int)_minDelay.TotalMilliseconds;
        var maxMs = (int)_maxDelay.TotalMilliseconds;
        int delayMs = minMs == maxMs && _randomFunc is null
            ? minMs
            : minMs + (_randomFunc?.Invoke(maxMs - minMs + 1) ?? new Random().Next(maxMs - minMs + 1));
        await _delayProvider.DelayAsync(TimeSpan.FromMilliseconds(delayMs), cancellationToken);
    }
}
