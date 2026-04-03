namespace ProcrastiN8.JustBecause;

/// <summary>
/// Executes all retries simultaneously, in superposition. The first successful result collapses reality;
/// all other attempts are quietly cancelled, as if they never happened.
/// </summary>
/// <remarks>
/// <para>
/// <see cref="RetryInSuperposition"/> is the quantum answer to the question: "What if we just tried
/// everything at once and waited to see what stuck?" This is also how most large organisations
/// approach strategic planning.
/// </para>
/// <para>
/// Note: the cost of running all attempts in parallel is a feature, not a flaw.
/// It demonstrates commitment to the outcome across all possible timelines simultaneously.
/// </para>
/// </remarks>
public static class RetryInSuperposition
{
    /// <summary>
    /// Executes all <paramref name="maxAttempts"/> in parallel. The first to succeed collapses
    /// reality and returns its result. All remaining attempts are discarded, their contributions
    /// noted in no commit history whatsoever.
    /// </summary>
    /// <typeparam name="T">The result type of the operation.</typeparam>
    /// <param name="operation">The operation to run in superposition across all timelines.</param>
    /// <param name="maxAttempts">Number of parallel attempts. Defaults to 3 (one per popular cloud provider).</param>
    /// <param name="logger">Optional logger for philosophical commentary on what just happened.</param>
    /// <param name="cancellationToken">Token to cancel all timelines at once.</param>
    /// <returns>The result from whichever timeline succeeded first.</returns>
    /// <exception cref="AggregateException">Thrown if all timelines fail simultaneously. Theoretically impossible; practically, here we are.</exception>
    public static async Task<T> ExecuteAsync<T>(
        Func<Task<T>> operation,
        int maxAttempts = 3,
        IProcrastiLogger? logger = null,
        CancellationToken cancellationToken = default)
    {
        if (maxAttempts < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(maxAttempts), "At least one timeline must be attempted.");
        }

        logger?.Info("[RetryInSuperposition] Collapsing {Count} parallel attempt(s) into a single observable outcome.", maxAttempts);

        var tasks = Enumerable
            .Range(0, maxAttempts)
            .Select(i =>
            {
                logger?.Debug("[RetryInSuperposition] Timeline {Index} initialized and running.", i + 1);
                try
                {
                    return operation();
                }
                catch (Exception ex)
                {
                    // The operation threw synchronously — wrap in a faulted task so WhenAny can handle it
                    return Task.FromException<T>(ex);
                }
            })
            .ToList();

        var exceptions = new List<Exception>();

        while (tasks.Count > 0)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var completed = await Task.WhenAny(tasks);
            tasks.Remove(completed);

            if (completed.IsCompletedSuccessfully)
            {
                logger?.Info("[RetryInSuperposition] Reality collapsed successfully. Discarding {Count} surviving timeline(s).", tasks.Count);
                return await completed;
            }

            if (completed.Exception is not null)
            {
                exceptions.AddRange(completed.Exception.InnerExceptions);
            }
        }

        logger?.Error("[RetryInSuperposition] All timelines failed simultaneously. This is theoretically improbable.");
        throw new AggregateException("All superposition attempts failed simultaneously. This is theoretically impossible, yet here we are.", exceptions);
    }

    /// <summary>
    /// Executes all <paramref name="maxAttempts"/> in parallel. The first to succeed collapses reality.
    /// </summary>
    /// <param name="operation">The async action to run across all timelines.</param>
    /// <param name="maxAttempts">Number of parallel attempts. Defaults to 3.</param>
    /// <param name="logger">Optional logger for inter-dimensional commentary.</param>
    /// <param name="cancellationToken">Token to cancel all timelines.</param>
    public static async Task ExecuteAsync(
        Func<Task> operation,
        int maxAttempts = 3,
        IProcrastiLogger? logger = null,
        CancellationToken cancellationToken = default)
    {
        await ExecuteAsync<int>(
            async () => { await operation(); return 0; },
            maxAttempts,
            logger,
            cancellationToken);
    }
}
