using ProcrastiN8.Common;
using ProcrastiN8.JustBecause;
using ProcrastiN8.LazyTasks;

namespace ProcrastiN8.Services;

/// <summary>
/// A procrastination strategy that waits until only the final fraction of the allotted window remains,
/// then executes the task in a highly productive panic.
/// </summary>
/// <remarks>
/// <para>
/// Studies show that the majority of productive work occurs in the last 10% of available time.
/// <see cref="LastMinuteStrategy"/> operationalises this insight by eliminating the inefficient
/// 90% that precedes it.
/// </para>
/// <para>
/// The strategy is not lazy. It is pacing itself strategically.
/// </para>
/// </remarks>
public class LastMinuteStrategy : ProcrastinationStrategyBase
{
    private static readonly string[] PanicRemarks =
    [
        "This is fine. Everything is fine.",
        "The deadline was always just a suggestion.",
        "Adrenaline-driven delivery is still delivery.",
        "We work best under pressure. This is pressure. This is best.",
        "Technical debt can be addressed in the next sprint.",
        "Ship it. We'll hotfix tomorrow.",
        "It compiled on the first try. That's a good sign.",
        "The tests pass locally.",
        "We can document this later. After the release. Definitely.",
        "An 11:58 PM commit counts as 'delivered today'.",
    ];

    /// <summary>The fraction of total window that triggers execution. Defaults to the last 10%.</summary>
    public const double DefaultLastFraction = 0.90;
    /// <summary>Total allotted window. Defaults to 2 seconds for test-friendly determinism.</summary>
    public static readonly TimeSpan DefaultTotalWindow = TimeSpan.FromSeconds(2);
    /// <summary>Per-cycle polling interval while waiting for the last minute.</summary>
    public static readonly TimeSpan DefaultPollingInterval = TimeSpan.FromMilliseconds(10);

    private readonly TimeSpan _totalWindow;
    private readonly double _lastFraction;
    private readonly TimeSpan _pollingInterval;
    private readonly IProcrastiLogger? _logger;

    /// <summary>
    /// Initializes a new instance of <see cref="LastMinuteStrategy"/>.
    /// </summary>
    /// <param name="totalWindow">Total allotted window. Execution occurs in the final <paramref name="lastFraction"/> of this window.</param>
    /// <param name="lastFraction">Fraction of <paramref name="totalWindow"/> that defines "the last minute". Defaults to 0.90 (last 10%).</param>
    /// <param name="pollingInterval">How frequently to check whether the last minute has arrived.</param>
    /// <param name="logger">Optional logger for increasingly urgent status updates.</param>
    public LastMinuteStrategy(
        TimeSpan? totalWindow = null,
        double lastFraction = DefaultLastFraction,
        TimeSpan? pollingInterval = null,
        IProcrastiLogger? logger = null)
    {
        _totalWindow = totalWindow ?? DefaultTotalWindow;
        _lastFraction = Math.Clamp(lastFraction, 0.0, 1.0);
        _pollingInterval = pollingInterval ?? DefaultPollingInterval;
        _logger = logger;
    }

    /// <inheritdoc />
    protected override async Task ExecuteCoreAsync(
        Func<Task> task,
        TimeSpan initialDelay,
        IExcuseProvider? excuseProvider,
        IDelayStrategy delayStrategy,
        IRandomProvider randomProvider,
        ITimeProvider timeProvider,
        CancellationToken cancellationToken)
    {
        var panicThreshold = StartUtc + TimeSpan.FromTicks((long)(_totalWindow.Ticks * _lastFraction));
        var hardDeadline = StartUtc + _totalWindow;

        _logger?.Info("[LastMinute] Panic threshold: {Threshold:HH:mm:ss.fff}. Hard deadline: {Deadline:HH:mm:ss.fff}.",
            panicThreshold.LocalDateTime, hardDeadline.LocalDateTime);

        while (!cancellationToken.IsCancellationRequested)
        {
            if (await CheckForExternalOverrideAsync(task)) { return; }

            var now = timeProvider.GetUtcNow();

            if (now >= panicThreshold || SafetyCapReached())
            {
                // The last minute has arrived. Commence productive panic.
                var panicRemark = PanicRemarks[randomProvider.GetRandom(PanicRemarks.Length)];
                _logger?.Info("[LastMinute] LAST MINUTE ACTIVATED. {Remark}", panicRemark);
                break;
            }

            var remaining = panicThreshold - now;
            _logger?.Debug("[LastMinute] Not yet. {Remaining:0.0}s until productive panic begins.", remaining.TotalSeconds);

            await InvokeExcuseAsync(excuseProvider);
            IncrementCycle();
            await delayStrategy.DelayAsync(_pollingInterval, _pollingInterval, cancellationToken: cancellationToken);
            await NotifyCycleAsync(ControlContext, cancellationToken);
        }

        if (cancellationToken.IsCancellationRequested)
        {
            return; // The deadline was cancelled. This has never resolved anything.
        }

        await task();
        MarkExecuted();
    }
}
