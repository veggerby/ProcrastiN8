namespace ProcrastiN8.Services;

/// <summary>
/// Optional extension interface exposing the last execution result for strategies that report metrics.
/// </summary>
public interface IResultReportingProcrastinationStrategy : IProcrastinationStrategy
{
    /// <summary>Gets the result captured during the last execution.</summary>
    ProcrastinationResult LastResult { get; }
}
