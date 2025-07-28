namespace ProcrastiN8.Extensions;

/// <summary>
/// The default, production-grade strategy for postponing <see cref="DateTime"/> values.
/// </summary>
public sealed class DefaultDateTimePostponeStrategy : IDateTimePostponeStrategy
{
    /// <summary>
    /// Singleton instance of the default strategy.
    /// </summary>
    public static DefaultDateTimePostponeStrategy Instance { get; } = new();

    private DefaultDateTimePostponeStrategy() { }

    /// <inheritdoc />
    public DateTime Postpone(DateTime original, TimeSpan postponeBy)
    {
        // The default strategy simply adds the duration, but could be replaced with something more arbitrary.
        return original.Add(postponeBy);
    }
}