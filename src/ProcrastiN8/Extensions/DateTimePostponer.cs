using ProcrastiN8.Common;

namespace ProcrastiN8.Extensions;

/// <summary>
/// Provides extension methods for <see cref="DateTime"/> to facilitate arbitrary, over-engineered postponement.
/// </summary>
public static class DateTimePostponer
{
    /// <summary>
    /// Returns a new <see cref="DateTime"/> postponed by the specified duration, using the provided strategy, excuse provider, and logger.
    /// </summary>
    /// <param name="dateTime">The original date and time.</param>
    /// <param name="postponeBy">The duration to postpone.</param>
    /// <param name="strategy">The strategy to determine how postponement is applied. If null, <see cref="DefaultDateTimePostponeStrategy"/> is used.</param>
    /// <param name="excuseProvider">An optional excuse provider for logging plausible justifications.</param>
    /// <param name="logger">An optional logger for audit trails.</param>
    /// <returns>A new <see cref="DateTime"/> postponed by the specified duration, or as determined by the strategy.</returns>
    public static DateTime Postpone(
        this DateTime dateTime,
        TimeSpan postponeBy,
        IDateTimePostponeStrategy? strategy = null,
        IExcuseProvider? excuseProvider = null,
        IProcrastiLogger? logger = null)
    {
        var actualStrategy = strategy ?? DefaultDateTimePostponeStrategy.Instance;
        var result = actualStrategy.Postpone(dateTime, postponeBy);

        if (excuseProvider is not null && logger is not null)
        {
            var excuse = excuseProvider.GetExcuse();
            logger.Info($"Postponed from {dateTime:o} to {result:o}: {excuse}");
        }

        return result;
    }

    /// <summary>
    /// Asynchronously returns a new <see cref="DateTime"/> postponed by the specified duration, simulating deliberation.
    /// </summary>
    /// <param name="dateTime">The original date and time.</param>
    /// <param name="postponeBy">The duration to postpone.</param>
    /// <param name="strategy">The strategy to determine how postponement is applied. If null, <see cref="DefaultDateTimePostponeStrategy"/> is used.</param>
    /// <param name="excuseProvider">An optional excuse provider for logging plausible justifications.</param>
    /// <param name="logger">An optional logger for audit trails.</param>
    /// <param name="cancellationToken">A token to cancel the postponement deliberation.</param>
    /// <returns>A task producing a new <see cref="DateTime"/> postponed by the specified duration, or as determined by the strategy.</returns>
    public static async Task<DateTime> PostponeAsync(
        this DateTime dateTime,
        TimeSpan postponeBy,
        IDateTimePostponeStrategy? strategy = null,
        IExcuseProvider? excuseProvider = null,
        IProcrastiLogger? logger = null,
        CancellationToken cancellationToken = default)
    {
        // Simulate deliberation delay
        await Task.Delay(TimeSpan.FromMilliseconds(137), cancellationToken);
        return dateTime.Postpone(postponeBy, strategy, excuseProvider, logger);
    }
}
