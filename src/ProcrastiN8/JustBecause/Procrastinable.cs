using ProcrastiN8.LazyTasks;

namespace ProcrastiN8.JustBecause;

/// <summary>
/// A deferred computation that pretends the delay was intentional.
/// </summary>
/// <remarks>
/// <para>
/// <see cref="Procrastinable{T}"/> is conceptually similar to <see cref="Lazy{T}"/>, except it adds
/// a brief deliberation window before evaluation, logs its intent with professional gravity, and caches
/// the result to avoid repeating the effort of getting around to it.
/// </para>
/// <para>
/// Once evaluated, the result is cached — not because of performance considerations, but because
/// doing it again would suggest the first time didn't count.
/// </para>
/// </remarks>
/// <typeparam name="T">The type of the value produced by eventually getting around to it.</typeparam>
public sealed class Procrastinable<T>
{
    private readonly Func<Task<T>> _factory;
    private readonly IDelayProvider _delayProvider;
    private readonly IRandomProvider _randomProvider;
    private readonly IProcrastiLogger? _logger;

    private Task<T>? _evaluationTask;

    // Minimum deliberation delay before committing to evaluation (ms)
    private const int MinDeliberationMs = 10;
    // Maximum deliberation delay before committing to evaluation (ms)
    private const int MaxDeliberationMs = 50;

    /// <summary>
    /// Gets whether this instance has been evaluated and the result is available.
    /// </summary>
    public bool IsEvaluated => _evaluationTask?.IsCompletedSuccessfully == true;

    /// <summary>
    /// Initializes a new instance of <see cref="Procrastinable{T}"/>.
    /// </summary>
    /// <param name="factory">The factory function to eventually, reluctantly invoke.</param>
    /// <param name="delayProvider">Optional delay provider for the deliberation window.</param>
    /// <param name="randomProvider">Optional random provider for deliberation duration.</param>
    /// <param name="logger">Optional logger for earnest status updates.</param>
    public Procrastinable(
        Func<Task<T>> factory,
        IDelayProvider? delayProvider = null,
        IRandomProvider? randomProvider = null,
        IProcrastiLogger? logger = null)
    {
        _factory = factory ?? throw new ArgumentNullException(nameof(factory));
        _delayProvider = delayProvider ?? new TaskDelayProvider();
        _randomProvider = randomProvider ?? RandomProvider.Default;
        _logger = logger;
    }

    /// <summary>
    /// Evaluates the deferred computation, after a brief period of apparent reflection.
    /// Subsequent calls return the cached result without further deliberation.
    /// </summary>
    /// <param name="cancellationToken">Token to cancel the evaluation, if the time still isn't right.</param>
    /// <returns>The result of the computation, eventually.</returns>
    public async Task<T> EvaluateAsync(CancellationToken cancellationToken = default)
    {
        if (_evaluationTask?.IsCompletedSuccessfully == true)
        {
            _logger?.Debug("[Procrastinable] Returning cached result. At least something worked out.");
            return await _evaluationTask;
        }

        var deliberationMs = _randomProvider.GetRandom(MinDeliberationMs, MaxDeliberationMs);
        _logger?.Info("[Procrastinable] After {DeliberationMs}ms of careful consideration, proceeding with evaluation.", deliberationMs);

        await _delayProvider.DelayAsync(TimeSpan.FromMilliseconds(deliberationMs), cancellationToken);

        _evaluationTask = _factory();
        var result = await _evaluationTask;

        _logger?.Info("[Procrastinable] Evaluation complete. Results filed for future reference.");
        return result;
    }

    /// <summary>
    /// Returns the evaluated value synchronously.
    /// Only safe to call after a successful <see cref="EvaluateAsync"/> — otherwise it throws,
    /// which is arguably the correct response to impatience.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if evaluation is not yet complete.</exception>
    public T Value
    {
        get
        {
            if (_evaluationTask?.IsCompletedSuccessfully != true)
            {
                throw new InvalidOperationException("The Procrastinable has not been evaluated yet. Good things come to those who await.");
            }

            return _evaluationTask.Result;
        }
    }
}
