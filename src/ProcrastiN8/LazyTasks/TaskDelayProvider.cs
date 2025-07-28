namespace ProcrastiN8.LazyTasks;

/// <summary>
/// Provides a production-grade, testable implementation of <see cref="IDelayProvider"/> using <see cref="Task.Delay"/>.
/// </summary>
public class TaskDelayProvider : IDelayProvider
{
    /// <inheritdoc />
    public virtual Task DelayAsync(TimeSpan delay, CancellationToken cancellationToken = default)
    {
        return Task.Delay(delay, cancellationToken);
    }
}
