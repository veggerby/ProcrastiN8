using ProcrastiN8.LazyTasks;

namespace ProcrastiN8.JustBecause;

/// <summary>
/// Schedules a task across all possible parallel universes simultaneously, per the many-worlds interpretation.
/// Each universe runs an independent attempt. The first to deliver a successful result collapses
/// into the prime timeline; all remaining universes are quietly decoherent.
/// </summary>
/// <remarks>
/// <para>
/// Hugh Everett III proposed that every quantum event spawns a branching universe. The
/// <see cref="ManyWorldsScheduler"/> takes this literally and spawns a configurable number of
/// parallel execution timelines. Unlike <see cref="RetryInSuperposition"/>, which implies
/// uncertainty about which retry will work, <see cref="ManyWorldsScheduler"/> is philosophically
/// certain that all timelines are equally real — it simply discards the less convenient ones.
/// </para>
/// <para>
/// Universes are independently seeded with a delay jitter so they don't all start simultaneously.
/// The universe that wins is the one that got there first, which is always the best universe.
/// </para>
/// </remarks>
public static class ManyWorldsScheduler
{
    /// <summary>
    /// Executes <paramref name="operation"/> across <paramref name="universeCount"/> parallel universes.
    /// The first successful result collapses reality; all other universes are abandoned.
    /// </summary>
    /// <typeparam name="T">The result type produced by each universe's attempt.</typeparam>
    /// <param name="operation">
    /// A factory accepting a universe index (0-based) and returning the operation to run in that universe.
    /// Each universe may behave differently based on its index — this is encouraged.
    /// </param>
    /// <param name="universeCount">Number of parallel universes to spawn. Defaults to 3.</param>
    /// <param name="jitterPerUniverse">Per-universe startup jitter to prevent identical simultaneous starts. Defaults to 5ms.</param>
    /// <param name="randomProvider">Random provider for jitter calculation.</param>
    /// <param name="delayProvider">Optional delay provider for universe startup jitter.</param>
    /// <param name="logger">Optional logger for timeline commentary.</param>
    /// <param name="cancellationToken">Token to collapse all universes simultaneously.</param>
    /// <returns>The result from whichever universe delivered first.</returns>
    /// <exception cref="AggregateException">Thrown if all universes fail. This should not happen. It has.</exception>
    public static async Task<T> ScheduleAsync<T>(
        Func<int, Task<T>> operation,
        int universeCount = 3,
        TimeSpan? jitterPerUniverse = null,
        IRandomProvider? randomProvider = null,
        IDelayProvider? delayProvider = null,
        IQuantumInterpretation? interpretation = null,
        IProcrastiLogger? logger = null,
        CancellationToken cancellationToken = default)
    {
        if (universeCount < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(universeCount), "At least one universe must exist. Zero universes is a philosophy, not a schedule.");
        }

        randomProvider ??= RandomProvider.Default;
        delayProvider ??= new TaskDelayProvider();
        interpretation ??= QuantumInterpretations.Copenhagen;
        var jitter = jitterPerUniverse ?? TimeSpan.FromMilliseconds(5);

        logger?.Info("[ManyWorlds] Spawning {Count} parallel universe(s) under the {Interpretation} interpretation. Each is equally real. Only one will matter.", universeCount, interpretation.Name);

        using var collapseSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

        var universes = Enumerable.Range(0, universeCount).Select(async i =>
        {
            // Each universe starts with a small, randomised delay to simulate the branching point.
            var startupMs = jitter.TotalMilliseconds > 0
                ? randomProvider.GetRandom(Math.Max(1, (i + 1) * (int)jitter.TotalMilliseconds))
                : 0;
            if (startupMs > 0)
            {
                await delayProvider.DelayAsync(TimeSpan.FromMilliseconds(startupMs), collapseSource.Token);
            }

            logger?.Debug("[ManyWorlds] Universe {Index} initialised. Beginning execution in this timeline.", i);
            return await operation(i);
        }).ToList();

        var exceptions = new List<Exception>();
        var remaining = universes.Cast<Task>().ToList();

        while (remaining.Count > 0)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var first = await Task.WhenAny(remaining);
            remaining.Remove(first);

            if (first is Task<T> typedFirst && first.IsCompletedSuccessfully)
            {
                var result = await typedFirst;

                if (interpretation.ParallelTimelinesAreReal)
                {
                    // Many-Worlds: all universes are real — do not cancel surviving timelines.
                    // Observe them to avoid unhandled exceptions, but let them complete.
                    logger?.Info("[ManyWorlds] Prime timeline resolved. {Remaining} parallel universe(s) continue to exist ({Interpretation}).", remaining.Count, interpretation.Name);
                    await Task.WhenAll(remaining.Select(t => t.ContinueWith(_ => { }, TaskContinuationOptions.None)));
                }
                else
                {
                    // Copenhagen et al.: collapse. Cancel and observe surviving universes.
                    logger?.Info("[ManyWorlds] Timeline collapsed ({Interpretation}). {Remaining} alternate universe(s) decoherent.", interpretation.Name, remaining.Count);
                    collapseSource.Cancel();
                    await Task.WhenAll(remaining.Select(t => t.ContinueWith(_ => { }, TaskContinuationOptions.None)));
                }

                return result;
            }

            if (first.Exception is not null)
            {
                foreach (var ex in first.Exception.InnerExceptions)
                {
                    if (ex is not OperationCanceledException)
                    {
                        exceptions.Add(ex);
                    }
                }
            }
        }

        throw new AggregateException("All parallel universes failed. This implies the task was not meant to be done in any timeline.", exceptions);
    }

    /// <summary>
    /// Executes <paramref name="operation"/> across <paramref name="universeCount"/> parallel universes.
    /// All universes run the same operation. First to succeed wins.
    /// </summary>
    /// <typeparam name="T">The result type.</typeparam>
    /// <param name="operation">The operation to run in every universe.</param>
    /// <param name="universeCount">Number of parallel universes. Defaults to 3.</param>
    /// <param name="jitterPerUniverse">Startup jitter per universe. Defaults to 5ms.</param>
    /// <param name="randomProvider">Random provider for jitter.</param>
    /// <param name="delayProvider">Optional delay provider.</param>
    /// <param name="interpretation">Optional quantum interpretation. Defaults to <see cref="QuantumInterpretations.Copenhagen"/>.</param>
    /// <param name="logger">Optional logger.</param>
    /// <param name="cancellationToken">Cancels all universes.</param>
    public static Task<T> ScheduleAsync<T>(
        Func<Task<T>> operation,
        int universeCount = 3,
        TimeSpan? jitterPerUniverse = null,
        IRandomProvider? randomProvider = null,
        IDelayProvider? delayProvider = null,
        IQuantumInterpretation? interpretation = null,
        IProcrastiLogger? logger = null,
        CancellationToken cancellationToken = default) =>
        ScheduleAsync<T>(
            _ => operation(),
            universeCount,
            jitterPerUniverse,
            randomProvider,
            delayProvider,
            interpretation,
            logger,
            cancellationToken);
}
