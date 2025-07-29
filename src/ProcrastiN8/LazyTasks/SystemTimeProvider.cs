namespace ProcrastiN8.LazyTasks;

/// <summary>
/// Default implementation of <see cref="ITimeProvider"/> using <see cref="DateTimeOffset.UtcNow"/>.
/// </summary>
public sealed class SystemTimeProvider : ITimeProvider
{
    public static readonly ITimeProvider Default = new SystemTimeProvider();

    /// <inheritdoc />
    public DateTimeOffset GetUtcNow() => DateTimeOffset.UtcNow;
}