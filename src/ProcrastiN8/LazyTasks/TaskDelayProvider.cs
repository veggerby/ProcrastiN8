namespace ProcrastiN8.LazyTasks;

/// <summary>
/// Provides a production-grade, testable implementation of <see cref="IDelayProvider"/> using <see cref="Task.Delay"/>.
/// </summary>
public class TaskDelayProvider(ITimeProvider? timeProvider = null) : IDelayProvider
{
    private readonly ITimeProvider _timeProvider = timeProvider ?? new SystemTimeProvider();

    /// <inheritdoc />
    public virtual Task DelayAsync(TimeSpan delay, CancellationToken cancellationToken = default)
    {
        return _timeProvider.DelayAsync(delay, cancellationToken);
    }
}