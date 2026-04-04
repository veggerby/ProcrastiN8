using ProcrastiN8.JustBecause;

namespace ProcrastiN8.Services;

/// <summary>
/// Defines how a delay evolves across procrastination cycles.
/// </summary>
public interface IPacingGrowthPolicy
{
    /// <summary>Calculates the next delay given the current delay and cycle count (zero-based).</summary>
    TimeSpan Next(TimeSpan currentDelay, int cycle, IRandomProvider randomProvider);
}

/// <summary>Default exponential-ish growth with bounded multiplicative jitter.</summary>
public sealed class DefaultPacingGrowthPolicy : IPacingGrowthPolicy
{
    public static readonly DefaultPacingGrowthPolicy Instance = new();

    public TimeSpan Next(TimeSpan currentDelay, int cycle, IRandomProvider randomProvider)
    {
        var jitter = 1.005 + randomProvider.GetDouble() * 0.15; // 0.5%–15% growth to keep tests brisk
        var next = TimeSpan.FromMilliseconds(currentDelay.TotalMilliseconds * jitter);
        return next;
    }
}