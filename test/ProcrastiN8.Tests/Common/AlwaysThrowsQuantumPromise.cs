using ProcrastiN8.JustBecause;

namespace ProcrastiN8.Tests.Common;

/// <summary>
/// A deterministic test double for IQuantumPromise that always throws the provided exception.
/// </summary>
public class AlwaysThrowsQuantumPromise<T>(Exception exception) : IQuantumPromise<T>
{
    private readonly Exception _exception = exception;

    public DateTimeOffset CreationTime => throw new NotImplementedException();

    public T Value => throw new NotImplementedException();

    public Task<T> ObserveAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromException<T>(_exception);
    }

    public override string ToString() => $"[AlwaysThrowsQuantumPromise: {_exception.GetType().Name}]";
}