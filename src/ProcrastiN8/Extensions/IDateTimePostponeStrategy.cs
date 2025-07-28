namespace ProcrastiN8.Extensions;

/// <summary>
/// Defines a strategy for arbitrarily postponing a <see cref="DateTime"/>.
/// </summary>
public interface IDateTimePostponeStrategy
{
    /// <summary>
    /// Calculates a new <see cref="DateTime"/> based on the original and a requested postponement.
    /// </summary>
    /// <param name="original">The original date and time.</param>
    /// <param name="postponeBy">The intended postponement duration.</param>
    /// <returns>A new <see cref="DateTime"/> reflecting the strategy's interpretation of postponement.</returns>
    DateTime Postpone(DateTime original, TimeSpan postponeBy);
}