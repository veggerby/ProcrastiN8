namespace ProcrastiN8.Services;

/// <summary>
/// Default factory mapping modes to concrete strategy implementations.
/// </summary>
public sealed class DefaultProcrastinationStrategyFactory : IProcrastinationStrategyFactory
{
    /// <inheritdoc />
    public IProcrastinationStrategy Create(ProcrastinationMode mode) => mode switch
    {
        ProcrastinationMode.MovingTarget => new MovingTargetStrategy(),
        ProcrastinationMode.InfiniteEstimation => new InfiniteEstimationStrategy(),
        ProcrastinationMode.WeekendFallback => new WeekendFallbackStrategy(),
        _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null)
    };
}