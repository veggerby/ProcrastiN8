namespace ProcrastiN8.TODOFramework;

/// <summary>
/// Defines a contract for executing actions that are never intended to be performed, in accordance with best practices for perpetual procrastination.
/// </summary>
/// <remarks>
/// This interface is designed for scenarios where the mere suggestion of execution is sufficient, and actual execution is to be indefinitely deferred.
/// </remarks>
public interface INeverDoExecutor
{
    /// <summary>
    /// Asynchronously never executes the specified action, optionally providing an excuse for the inaction.
    /// </summary>
    /// <param name="action">The action that will not be executed.</param>
    /// <param name="excuse">An optional excuse to justify the lack of execution.</param>
    /// <param name="cancellationToken">A token to cancel the never-execution, should the need to not act become urgent.</param>
    /// <returns>A task representing the never-executed operation.</returns>
    /// <remarks>
    /// This method is intended for use in workflows where the appearance of intent is valued over actual progress.
    /// </remarks>
    Task NeverAsync(Func<Task> action, string? excuse = null, CancellationToken cancellationToken = default);
}
