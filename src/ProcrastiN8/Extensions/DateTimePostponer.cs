namespace ProcrastiN8.Extensions;

/// <summary>
/// Provides extension methods for <see cref="DateTime"/> to facilitate arbitrary postponement.
/// </summary>
public static class DateTimePostponer
{
    /// <summary>
    /// Returns a new <see cref="DateTime"/> arbitrarily postponed by the specified duration.
    /// </summary>
    /// <param name="dateTime">The original date and time.</param>
    /// <param name="postponeBy">The duration to postpone.</param>
    /// <returns>A new <see cref="DateTime"/> postponed by the specified duration.</returns>
    public static DateTime Postpone(this DateTime dateTime, TimeSpan postponeBy)
    {
        return dateTime.Add(postponeBy);
    }
}
