namespace ProcrastiN8.LazyTasks;

using ProcrastiN8.JustBecause;
using ProcrastiN8; // For IProcrastiLogger

/// <summary>
/// Provides a default, configurable delay strategy for simulating arbitrary stalling with optional randomness.
/// </summary>
/// <remarks>
/// This strategy introduces a fixed or random delay, suitable for simulating indecision or network latency.
/// The implementation intentionally abstracts time and randomness to enable deterministic unit testing.
/// </remarks>
/// <param name="minDelay">The minimum delay duration (defaults to 1 second).</param>
/// <param name="maxDelay">The maximum delay duration (defaults to 3 seconds).</param>
/// <param name="randomProvider">Random provider for selecting a delay in the configured range.</param>
/// <param name="delayProvider">Underlying delay provider implementation.</param>
/// <param name="logger">Optional logger used for excessively earnest trace commentary.</param>
public class DefaultDelayStrategy(
    TimeSpan? minDelay = null,
    TimeSpan? maxDelay = null,
    IRandomProvider? randomProvider = null,
    IDelayProvider? delayProvider = null,
    IProcrastiLogger? logger = null) : IDelayStrategy
{
    private readonly TimeSpan _minDelay = minDelay ?? TimeSpan.FromSeconds(1);
    private readonly TimeSpan _maxDelay = maxDelay ?? TimeSpan.FromSeconds(3);
    private readonly IRandomProvider _randomProvider = randomProvider ?? RandomProvider.Default;
    private readonly IDelayProvider _delayProvider = delayProvider ?? new TaskDelayProvider();
    private readonly IProcrastiLogger? _logger = logger;

    /// <inheritdoc />
    public async Task DelayAsync(TimeSpan? minDelay = null, TimeSpan? maxDelay = null, Func<double, bool>? beforeCallback = null, CancellationToken cancellationToken = default)
    {
        var minMs = (int?)minDelay?.TotalMilliseconds ?? (int)_minDelay.TotalMilliseconds;
        var maxMs = (int?)maxDelay?.TotalMilliseconds ?? (int)_maxDelay.TotalMilliseconds;

        if (minMs < 0 || maxMs < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(minDelay), "Delay bounds must be non-negative.");
        }
        if (minMs > maxMs)
        {
            // Swap defensively – the caller is clearly distracted.
            (minMs, maxMs) = (maxMs, minMs);
        }

        int delayMs = minMs == maxMs ? minMs : _randomProvider.GetRandom(minMs, maxMs + 1);

        _logger?.Debug($"Applying procrastination delay of {delayMs}ms (range {minMs}-{maxMs}ms).");

        if (beforeCallback is not null && !beforeCallback(delayMs))
        {
            _logger?.Info("Delay skipped by before-callback predicate – deferral opportunity tragically lost.");
            return;
        }

        await _delayProvider.DelayAsync(TimeSpan.FromMilliseconds(delayMs), cancellationToken);
    }
}