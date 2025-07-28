using ProcrastiN8.Common;
using ProcrastiN8.LazyTasks;

namespace ProcrastiN8.TODOFramework;

/// <summary>
/// Executes actions that are never meant to be completed, in strict accordance with the TODO philosophy.
/// </summary>
/// <remarks>
/// The <see cref="NeverDoExecutor"/> provides a safe, enterprise-grade mechanism for ensuring that certain actions are never performed, regardless of demand or urgency.
/// </remarks>
public class NeverDoExecutor(IProcrastiLogger? logger = null, IExcuseProvider? excuseProvider = null, IDelayStrategy? delayStrategy = null) : INeverDoExecutor
{
    private readonly IProcrastiLogger? _logger = logger;
    private readonly IExcuseProvider? _excuseProvider = excuseProvider;
    private readonly IDelayStrategy? _delayStrategy = delayStrategy;

    /// <summary>
    /// Attempts to execute the provided action, but never actually does so.
    /// </summary>
    /// <param name="action">The action to never execute.</param>
    /// <param name="excuse">An optional excuse to log for not executing.</param>
    /// <param name="cancellationToken">A token to cancel the non-execution process.</param>
    public Task NeverAsync(Func<Task> action, string? excuse = null, CancellationToken cancellationToken = default)
    {
        if (action is null)
            throw new ArgumentNullException(nameof(action));

        var reason = excuse ?? _excuseProvider?.GetExcuse() ?? "Deferred indefinitely.";
        _logger?.Info($"Never executing action: {reason}");

        // Return a Task that never completes, unless cancellation is requested.
        var tcs = new TaskCompletionSource<object?>();
        cancellationToken.Register(() => tcs.TrySetCanceled(cancellationToken));
        return tcs.Task;
    }
}