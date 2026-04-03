namespace ProcrastiN8.Services;

public enum ProcrastinationMode
{
    MovingTarget,
    InfiniteEstimation,
    WeekendFallback,

    /// <summary>
    /// Convenes an imaginary stakeholder committee every cycle. Consensus is perpetually deferred.
    /// </summary>
    CommitteeReview,

    /// <summary>
    /// Always discovers one more critical open question that must be resolved before work can begin.
    /// </summary>
    AnalysisParalysis,
}