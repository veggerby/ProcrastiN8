namespace ProcrastiN8.Services;

/// <summary>
/// Determines whether a ceiling has been reached for a procrastination strategy.
/// </summary>
public interface IDelayCeilingPolicy
{
    /// <summary>Returns true when the strategy should stop deferring.</summary>
    bool ShouldCease(TimeSpan currentDelay, int cycles, TimeSpan totalElapsed);
}

/// <summary>Default ceiling: delay >= 1 hour OR 20 cycles (kept intentionally modest for test determinism).</summary>
public sealed class DefaultDelayCeilingPolicy : IDelayCeilingPolicy
{
    public static readonly DefaultDelayCeilingPolicy Instance = new();
    public bool ShouldCease(TimeSpan currentDelay, int cycles, TimeSpan totalElapsed) => currentDelay >= TimeSpan.FromHours(1) || cycles >= 20;
}
