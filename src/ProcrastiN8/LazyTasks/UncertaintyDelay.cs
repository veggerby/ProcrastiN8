using System.Diagnostics;
using ProcrastiN8.Common;
using ProcrastiN8.JustBecause;
using ProcrastiN8.Metrics;
using ProcrastiN8.Services;

namespace ProcrastiN8.LazyTasks;

/// <summary>
/// Provides a delay that changes unpredictably each time it is checked.
/// </summary>
/// <remarks>
/// This delay is designed to simulate the uncertainty of procrastination,
/// ensuring that no two checks yield the same result.
/// </remarks>
public static class UncertaintyDelay
{
    private static readonly ActivitySource ActivitySource = new("ProcrastiN8.LazyTasks.UncertaintyDelay");
    private static readonly TaskDelayProvider SharedDelayProvider = new();
    private static readonly CommentaryService CommentaryService = new();
    private static readonly ExcuseService ExcuseService = new();
    private static IRandomProvider _randomProvider = RandomProvider.Default;
    private static IProcrastiLogger _defaultLogger = new DefaultLogger();

    /// <summary>
    /// Allows overriding the default random provider and logger for all future calls (primarily for tests / instrumentation alignment).
    /// </summary>
    public static void SetDefaults(IRandomProvider? randomProvider = null, IProcrastiLogger? logger = null)
    {
        if (randomProvider is not null)
        {
            _randomProvider = randomProvider;
        }

        if (logger is not null)
        {
            _defaultLogger = logger;
        }
    }
    /// <summary>
    /// The minimum number of delay rounds to perform.
    /// </summary>
    public const int MinRounds = 2;

    /// <summary>
    /// The maximum number of delay rounds to perform.
    /// </summary>
    public const int MaxRounds = 5;

    /// <summary>
    /// Waits for one or more random durations up to the specified maximum delay.
    /// </summary>
    /// <param name="maxDelay">The maximum delay duration per round.</param>
    /// <param name="rounds">
    /// Number of delay rounds to perform. If null or less than 1, a random count (2–5) will be used.
    /// </param>
    /// <param name="randomProvider">The random provider used to determine delay duration and rounds.</param>
    /// <param name="excuseProvider">Optional excuse provider for logging procrastination reasons.</param>
    /// <param name="cancellationToken">A token to cancel the delay sequence.</param>
    /// <param name="delayProvider">The provider used to introduce delays.</param>
    /// <param name="logger">Optional logger for tracking delay execution.</param>
    /// <returns>A task that completes after one or more random delays.</returns>
    /// <remarks>
    /// The actual number of delay rounds to execute, determined by the provided or default random provider.
    /// </remarks>
    public static async Task WaitAsync(
        TimeSpan maxDelay,
        int? rounds = null,
        IRandomProvider? randomProvider = null,
        IExcuseProvider? excuseProvider = null,
        IDelayProvider? delayProvider = null,
        IProcrastiLogger? logger = null,
        bool enableCommentary = false,
        CancellationToken cancellationToken = default)
    {
        if (maxDelay <= TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(maxDelay), "Maximum delay must be greater than zero.");
        }

        randomProvider ??= _randomProvider;
        delayProvider ??= SharedDelayProvider;
        logger ??= _defaultLogger;

        int actualRounds = rounds.HasValue && rounds.Value > 0
            ? rounds.Value
            : randomProvider.GetRandom(MinRounds, MaxRounds + 1);

        using var activity = ActivitySource.StartActivity("UncertaintyDelay.WaitAsync", ActivityKind.Internal);
        activity?.SetTag("uncertainty.maxDelay.ms", maxDelay.TotalMilliseconds);
        activity?.SetTag("uncertainty.rounds.requested", rounds ?? 0);
        activity?.SetTag("uncertainty.rounds.actual", actualRounds);

        double totalDelayMs = 0;
        for (int i = 0; i < actualRounds; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var delay = TimeSpan.FromMilliseconds(randomProvider.GetDouble() * maxDelay.TotalMilliseconds);
            totalDelayMs += delay.TotalMilliseconds;

            string prefix = $"[UncertaintyDelay] Round {i + 1}/{actualRounds}";

            if (excuseProvider is not null)
            {
                var excuse = await excuseProvider.GetExcuseAsync() ?? "Uncertainty is high today.";
                logger.Info($"{prefix}: {excuse} Delaying for {delay.TotalMilliseconds:F2} ms.");
                ProcrastinationMetrics.ExcusesGenerated.Add(1);
                activity?.AddEvent(new ActivityEvent("uncertainty.excuse", tags: new ActivityTagsCollection
                {
                    {"excuse", excuse },
                    {"round", i + 1 }
                }));
            }
            else
            {
                // Fall back to synchronous excuse service for parity with other procrastination primitives.
                var generatedExcuse = ExcuseService.GenerateExcuse();
                logger.Info($"{prefix}: {generatedExcuse} (fallback) Delaying for {delay.TotalMilliseconds:F2} ms.");
                ProcrastinationMetrics.ExcusesGenerated.Add(1);
                activity?.AddEvent(new ActivityEvent("uncertainty.excuse", tags: new ActivityTagsCollection
                {
                    {"excuse", generatedExcuse },
                    {"round", i + 1 }
                }));
            }

            if (enableCommentary && i > 0 && (i % 2 == 0))
            {
                CommentaryService.LogRandomRemark();
                ProcrastinationMetrics.CommentaryTotal.Add(1);
            }

            try
            {
                await delayProvider.DelayAsync(delay, cancellationToken);
                ProcrastinationMetrics.DelaysTotal.Add(1);
                ProcrastinationMetrics.TotalTimeProcrastinated.Add((long)(delay.TotalSeconds),
                    KeyValuePair.Create<string, object?>("reason", "uncertainty-delay"));
            }
            catch (TaskCanceledException)
            {
                logger.Warn("[UncertaintyDelay] Delay was canceled during round execution.");
                throw;
            }
        }

        activity?.SetTag("uncertainty.totalDelay.ms", totalDelayMs);
    }
}