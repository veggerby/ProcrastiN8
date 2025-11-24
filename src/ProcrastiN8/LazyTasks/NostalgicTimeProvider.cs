namespace ProcrastiN8.LazyTasks;

/// <summary>
/// A time provider that believes it is perpetually a specific year, defaulting to 2019.
/// </summary>
/// <remarks>
/// This provider offers a stable, comforting alternate reality where the current year never changes.
/// All timestamps are anchored to the nostalgic year, with time progressing normally within that year
/// but never advancing beyond December 31st. This is ideal for avoiding the passage of time entirely
/// while maintaining the illusion of temporal consistency.
/// </remarks>
public sealed class NostalgicTimeProvider : ITimeProvider
{
    private readonly int _nostalgicYear;
    private readonly ITimeProvider _baseTimeProvider;
    private readonly DateTimeOffset _anchorPoint;

    /// <summary>
    /// Initializes a new instance of the <see cref="NostalgicTimeProvider"/> class.
    /// </summary>
    /// <param name="nostalgicYear">The year this provider believes it always is. Defaults to 2019.</param>
    /// <param name="baseTimeProvider">The underlying time provider to wrap. If null, uses <see cref="SystemTimeProvider"/>.</param>
    /// <remarks>
    /// 2019 is chosen as the default nostalgic year because it represents the last moment before
    /// everything became complicated. Alternative years may be specified for those with different
    /// temporal preferences.
    /// </remarks>
    public NostalgicTimeProvider(int nostalgicYear = 2019, ITimeProvider? baseTimeProvider = null)
    {
        if (nostalgicYear < 1900 || nostalgicYear > 2100)
        {
            throw new ArgumentOutOfRangeException(nameof(nostalgicYear), 
                "Nostalgic year must be between 1900 and 2100. We have limits to our temporal denial.");
        }

        _nostalgicYear = nostalgicYear;
        _baseTimeProvider = baseTimeProvider ?? SystemTimeProvider.Default;
        
        // Anchor point: when this provider was created in real time
        _anchorPoint = _baseTimeProvider.GetUtcNow();
    }

    /// <summary>
    /// Returns the current UTC timestamp, permanently set within the nostalgic year.
    /// </summary>
    /// <returns>A timestamp that exists in the configured nostalgic year.</returns>
    /// <remarks>
    /// The returned timestamp maintains the same month, day, hour, minute, and second as the real current time,
    /// but the year is always the nostalgic year. If real time would exceed December 31st of the nostalgic year,
    /// time loops back to January 1st, creating a comfortable temporal recursion.
    /// </remarks>
    public DateTimeOffset GetUtcNow()
    {
        var realNow = _baseTimeProvider.GetUtcNow();
        
        // Calculate elapsed time since anchor point
        var elapsed = realNow - _anchorPoint;
        
        // Start from January 1st of the nostalgic year
        var nostalgicStart = new DateTimeOffset(_nostalgicYear, 1, 1, 0, 0, 0, TimeSpan.Zero);
        
        // Add elapsed time
        var nostalgicNow = nostalgicStart + elapsed;
        
        // If we've exceeded the nostalgic year, loop back within the year
        var yearEnd = new DateTimeOffset(_nostalgicYear, 12, 31, 23, 59, 59, 999, TimeSpan.Zero);
        var yearStart = new DateTimeOffset(_nostalgicYear, 1, 1, 0, 0, 0, TimeSpan.Zero);
        var yearDuration = yearEnd - yearStart;
        
        while (nostalgicNow > yearEnd)
        {
            var overshoot = nostalgicNow - yearEnd;
            
            // Loop back to start of year, preserving the overshoot modulo year duration
            var loopedOffset = TimeSpan.FromTicks(overshoot.Ticks % yearDuration.Ticks);
            nostalgicNow = yearStart + loopedOffset;
        }

        return nostalgicNow;
    }

    /// <summary>
    /// Gets the nostalgic year that this provider is permanently set to.
    /// </summary>
    public int NostalgicYear => _nostalgicYear;

    /// <summary>
    /// Determines whether the current nostalgic time has looped at least once.
    /// </summary>
    /// <returns><c>true</c> if time has looped; otherwise, <c>false</c>.</returns>
    public bool HasLooped()
    {
        var realNow = _baseTimeProvider.GetUtcNow();
        var elapsed = realNow - _anchorPoint;
        var yearEnd = new DateTimeOffset(_nostalgicYear, 12, 31, 23, 59, 59, TimeSpan.Zero);
        var yearStart = new DateTimeOffset(_nostalgicYear, 1, 1, 0, 0, 0, TimeSpan.Zero);
        var yearDuration = yearEnd - yearStart;
        
        return elapsed > yearDuration;
    }
}
