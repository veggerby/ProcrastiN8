namespace ProcrastiN8.LazyTasks;

/// <summary>
/// Provides a production-grade, testable implementation of <see cref="IDelayProvider"/> using <see cref="Task.Delay"/>.
/// </summary>
public class TaskDelayProvider : IDelayProvider
{
    /// <inheritdoc />
    public Task DelayAsync(TimeSpan delay, CancellationToken cancellationToken = default)
        => Task.Delay(delay, cancellationToken);
}