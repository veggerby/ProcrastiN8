namespace ProcrastiN8.JustBecause;

/// <summary>
/// Exception thrown when fate rules against successful execution.
/// </summary>
public sealed class QuantumUncertaintyException : Exception
{
    /// <summary>The success probability that was offered and declined by the universe.</summary>
    public double OfferedProbability { get; }

    /// <inheritdoc cref="QuantumUncertaintyException"/>
    public QuantumUncertaintyException(double offeredProbability)
        : base($"The universe declined. Success probability was {offeredProbability:P0} — which was apparently too optimistic.")
    {
        OfferedProbability = offeredProbability;
    }
}
