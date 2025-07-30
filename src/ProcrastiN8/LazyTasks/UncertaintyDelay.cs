using ProcrastiN8.Common;
using ProcrastiN8.JustBecause;

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
        CancellationToken cancellationToken = default)
    {
        if (maxDelay <= TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(maxDelay), "Maximum delay must be greater than zero.");
        }

        randomProvider ??= RandomProvider.Default;
        delayProvider ??= new TaskDelayProvider();
        logger ??= new DefaultLogger();

        int actualRounds = rounds.HasValue && rounds.Value > 0
            ? rounds.Value
            : randomProvider.GetRandom(MinRounds, MaxRounds + 1);

        for (int i = 0; i < actualRounds; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var delay = TimeSpan.FromMilliseconds(randomProvider.GetDouble() * maxDelay.TotalMilliseconds);

            string prefix = $"[UncertaintyDelay] Round {i + 1}/{actualRounds}";

            if (excuseProvider is not null)
            {
                var excuse = await excuseProvider.GetExcuseAsync() ?? "Uncertainty is high today.";
                logger.Info($"{prefix}: {excuse} Delaying for {delay.TotalMilliseconds:F2} ms.");
            }
            else
            {
                logger.Info($"{prefix}: Delaying for {delay.TotalMilliseconds:F2} ms.");
            }

            try
            {
                await delayProvider.DelayAsync(delay, cancellationToken);
            }
            catch (TaskCanceledException)
            {
                logger.Warn("[UncertaintyDelay] Delay was canceled during round execution.");
                throw;
            }
        }
    }
}