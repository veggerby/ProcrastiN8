namespace ProcrastiN8.JustBecause;

/// <summary>
/// Executes an operation that may or may not succeed, governed entirely by fate (and a configurable probability).
/// </summary>
/// <remarks>
/// <para>
/// <see cref="ProbabilityOfSuccess"/> is the enterprise-grade answer to the question:
/// "What if we added a success rate slider?" It is also the honest answer to the question:
/// "How often does this actually work?"
/// </para>
/// <para>
/// A success probability of 1.0 means the operation always succeeds.
/// A success probability of 0.0 means you are testing your error handling.
/// Any value in between simulates reality.
/// </para>
/// </remarks>
public static class ProbabilityOfSuccess
{
    /// <summary>
    /// Executes the given operation with a configurable probability of succeeding.
    /// On failure, throws <see cref="QuantumUncertaintyException"/> before the operation runs.
    /// </summary>
    /// <typeparam name="T">The result type of the operation.</typeparam>
    /// <param name="operation">The operation to potentially execute.</param>
    /// <param name="successProbability">
    /// Probability between 0.0 (always fail) and 1.0 (always succeed).
    /// Defaults to 0.5, because on average, things are fine.
    /// </param>
    /// <param name="randomProvider">Injectable random source. Defaults to <see cref="RandomProvider.Default"/>.</param>
    /// <param name="cancellationToken">Token to cancel before fate can decide.</param>
    /// <returns>The result of the operation, if fate permits.</returns>
    /// <exception cref="QuantumUncertaintyException">Thrown when fate declines to cooperate.</exception>
    public static async Task<T> ExecuteAsync<T>(
        Func<Task<T>> operation,
        double successProbability = 0.5,
        IRandomProvider? randomProvider = null,
        CancellationToken cancellationToken = default)
    {
        if (successProbability is < 0.0 or > 1.0)
        {
            throw new ArgumentOutOfRangeException(nameof(successProbability), "Probability must be between 0.0 and 1.0.");
        }

        randomProvider ??= RandomProvider.Default;

        cancellationToken.ThrowIfCancellationRequested();

        if (randomProvider.GetDouble() >= successProbability)
        {
            throw new QuantumUncertaintyException(successProbability);
        }

        return await operation();
    }

    /// <summary>
    /// Executes the given action with a configurable probability of succeeding.
    /// On failure, throws <see cref="QuantumUncertaintyException"/> before the action runs.
    /// </summary>
    /// <param name="operation">The async action to potentially execute.</param>
    /// <param name="successProbability">Probability between 0.0 and 1.0. Defaults to 0.5.</param>
    /// <param name="randomProvider">Injectable random source.</param>
    /// <param name="cancellationToken">Token to cancel before fate decides.</param>
    /// <exception cref="QuantumUncertaintyException">Thrown when fate declines to cooperate.</exception>
    public static async Task ExecuteAsync(
        Func<Task> operation,
        double successProbability = 0.5,
        IRandomProvider? randomProvider = null,
        CancellationToken cancellationToken = default)
    {
        await ExecuteAsync<int>(
            async () => { await operation(); return 0; },
            successProbability,
            randomProvider,
            cancellationToken);
    }
}
