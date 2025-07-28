using ProcrastiN8.JustBecause;

namespace ProcrastiN8.Tests.Common;

/// <summary>
/// A deterministic test double for IQuantumPromise, always returns the provided value instantly.
/// </summary>
public class PredictableQuantumPromise<T>(T value) : IQuantumPromise<T>
{
    private readonly T _value = value;

    public Task<T> ObserveAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_value);
    }

    public override string ToString() => $"[PredictableQuantumPromise: {_value}]";
}