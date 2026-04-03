namespace ProcrastiN8.JustBecause;

/// <summary>
/// A cancellation token that aborts your task the moment it becomes important.
/// </summary>
/// <remarks>
/// <para>
/// The <see cref="QuantumAbortToken"/> monitors the perceived importance of the work and
/// triggers cancellation probabilistically when importance is observed. The more important
/// the task, the higher the likelihood of immediate cancellation.
/// </para>
/// <para>
/// This is not a bug. This is by design.
/// </para>
/// </remarks>
public sealed class QuantumAbortToken : IDisposable
{
    private readonly CancellationTokenSource _cts = new();
    private readonly IRandomProvider _randomProvider;
    private readonly double _baseCancellationProbability;
    private bool _disposed;

    /// <summary>
    /// Gets the underlying <see cref="CancellationToken"/> that will be triggered by quantum events.
    /// </summary>
    public CancellationToken Token => _cts.Token;

    /// <summary>
    /// Gets whether this token has been aborted.
    /// </summary>
    public bool IsAborted => _cts.IsCancellationRequested;

    /// <summary>
    /// Initializes a new instance of <see cref="QuantumAbortToken"/>.
    /// </summary>
    /// <param name="baseCancellationProbability">
    /// Base probability (0.0–1.0) that a task is cancelled when its importance is observed.
    /// Defaults to 0.3 — because 30% of the time, the timing is always wrong.
    /// </param>
    /// <param name="randomProvider">Injectable random source for deterministic testing.</param>
    public QuantumAbortToken(double baseCancellationProbability = 0.3, IRandomProvider? randomProvider = null)
    {
        if (baseCancellationProbability is < 0.0 or > 1.0)
        {
            throw new ArgumentOutOfRangeException(nameof(baseCancellationProbability), "Probability must be between 0.0 and 1.0.");
        }

        _baseCancellationProbability = baseCancellationProbability;
        _randomProvider = randomProvider ?? RandomProvider.Default;
    }

    /// <summary>
    /// Observes the task's importance level.
    /// The more important the task is declared, the higher the probability of immediate cancellation.
    /// </summary>
    /// <param name="importance">
    /// A multiplier for cancellation probability (1.0 = base rate, 2.0 = twice as likely, etc.).
    /// Because if it was truly critical, it would have been cancelled by now.
    /// </param>
    public void ObserveImportance(double importance = 1.0)
    {
        if (_disposed || _cts.IsCancellationRequested)
        {
            return;
        }

        var effectiveProbability = Math.Min(1.0, _baseCancellationProbability * importance);

        if (_randomProvider.GetDouble() < effectiveProbability)
        {
            _cts.Cancel();
        }
    }

    /// <summary>
    /// Immediately aborts the task. No observation required.
    /// For when you already know this was a bad idea.
    /// </summary>
    public void AbortImmediately() => _cts.Cancel();

    /// <inheritdoc />
    public void Dispose()
    {
        if (!_disposed)
        {
            _cts.Dispose();
            _disposed = true;
        }
    }
}
