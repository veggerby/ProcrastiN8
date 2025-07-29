namespace ProcrastiN8.LazyTasks;

/// <summary>
/// Default implementation of <see cref="ITimeProvider"/> using <see cref="Task.Delay"/>.
/// </summary>
public sealed class SystemTimeProvider : ITimeProvider
{
    /// <inheritdoc />
    public Task DelayAsync(TimeSpan delay, CancellationToken cancellationToken = default)
        => Task.Delay(delay, cancellationToken);

    /// <inheritdoc />
    public DateTimeOffset GetUtcNow() => DateTimeOffset.UtcNow;
}