namespace ProcrastiN8.Services;

/// <summary>
/// Defines a pipeline component that can wrap procrastination strategy execution for cross-cutting concerns.
/// </summary>
public interface IProcrastinationMiddleware
{
    /// <summary>Invoked with the current execution context and the next delegate in the chain.</summary>
    Task InvokeAsync(ProcrastinationExecutionContext context, Func<Task> next, CancellationToken cancellationToken);
}

/// <summary>Execution context flowing through middleware.</summary>
public sealed class ProcrastinationExecutionContext
{
    internal ProcrastinationExecutionContext(ProcrastinationMode mode, Guid correlationId) { Mode = mode; CorrelationId = correlationId; }
    public ProcrastinationMode Mode { get; }
    public Guid CorrelationId { get; }
    public ProcrastinationResult? Result { get; internal set; }
}
