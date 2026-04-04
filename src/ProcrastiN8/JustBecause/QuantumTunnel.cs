namespace ProcrastiN8.JustBecause;

/// <summary>
/// Allows tasks to pass through exception barriers they have absolutely no right to cross.
/// </summary>
/// <remarks>
/// <para>
/// In classical mechanics, a particle encountering a potential barrier it lacks the energy to
/// surmount will stop. In quantum mechanics, there is a nonzero probability that it tunnels
/// through anyway. <see cref="QuantumTunnel"/> applies this principle to software development,
/// where the barrier is a thrown exception and the tunnel is a configurable fallback value.
/// </para>
/// <para>
/// This is empirically different from a try/catch. A try/catch acknowledges the exception.
/// A <see cref="QuantumTunnel"/> simply proceeds as though the exception's classical authority
/// does not apply in this dimension.
/// </para>
/// </remarks>
public static class QuantumTunnel
{
    /// <summary>
    /// Attempts to execute <paramref name="operation"/>. If it throws, the exception is
    /// observed, noted in the logs, and then ignored — because tunnelling is probabilistic
    /// and this particular particle made it through.
    /// </summary>
    /// <typeparam name="T">The result type of the operation.</typeparam>
    /// <param name="operation">The operation that may or may not be stopped by thrown exceptions.</param>
    /// <param name="fallback">The value to return when the operation's exception barrier is encountered.</param>
    /// <param name="tunnelingProbability">Probability (0–1) that tunnelling succeeds. Defaults to 1.0 (always tunnels).
    /// When set below 1.0, there is a chance the exception is re-thrown — simulating partial barrier penetration.</param>
    /// <param name="randomProvider">Random provider for tunnelling probability evaluation.</param>
    /// <param name="interpretation">
    /// Optional quantum interpretation governing barrier penetration.
    /// Defaults to <see cref="QuantumInterpretations.Copenhagen"/>.
    /// When an interpretation has <see cref="IQuantumInterpretation.TunnellingPermitted"/> set to <c>false</c>,
    /// tunnelling probability is forced to 0 and all exceptions are re-thrown with classical authority.
    /// The interpretation may also modify the effective tunnelling probability via
    /// <see cref="IQuantumInterpretation.InterpretProbability"/>.
    /// </param>
    /// <param name="logger">Optional logger for tunnelling event commentary.</param>
    /// <param name="cancellationToken">Token to cancel the operation before tunnelling is needed.</param>
    /// <returns>The result of the operation, or <paramref name="fallback"/> if the barrier was tunnelled.</returns>
    public static async Task<T> TunnelAsync<T>(
        Func<Task<T>> operation,
        T fallback = default!,
        double tunnelingProbability = 1.0,
        IRandomProvider? randomProvider = null,
        IQuantumInterpretation? interpretation = null,
        IProcrastiLogger? logger = null,
        CancellationToken cancellationToken = default)
    {
        randomProvider ??= RandomProvider.Default;
        interpretation ??= QuantumInterpretations.Copenhagen;
        var statedProbability = Math.Clamp(tunnelingProbability, 0.0, 1.0);

        try
        {
            return await operation();
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            // Cancellation is not a barrier. It is a decision. Honour it.
            throw;
        }
        catch (Exception ex)
        {
            double effectiveProbability;
            if (!interpretation.TunnellingPermitted)
            {
                logger?.Debug(
                    "[QuantumTunnel] Interpretation '{Interpretation}' disallows tunnelling. All barriers retain classical authority.",
                    interpretation.Name);
                effectiveProbability = 0.0;
            }
            else
            {
                // Draw once — the same sample both informs the interpretation's probability adjustment
                // and determines the tunnelling outcome. No disconnect between adjustment and decision.
                var sample = randomProvider.GetDouble();
                effectiveProbability = interpretation.InterpretProbability(sample, statedProbability);
            }

            var tunnelled = effectiveProbability >= 1.0 || randomProvider.GetDouble() < effectiveProbability;

            if (tunnelled)
            {
                logger?.Info(
                    "[QuantumTunnel] Barrier '{ExceptionType}' encountered and tunnelled ({Interpretation}). Proceeding with fallback value. Classical physics need not apply.",
                    ex.GetType().Name, interpretation.Name);
                return fallback;
            }

            logger?.Warn(
                "[QuantumTunnel] Tunnelling failed ({Probability:P0} probability, {Interpretation}). The exception '{ExceptionType}' retains classical authority in this timeline.",
                effectiveProbability, interpretation.Name, ex.GetType().Name);
            throw;
        }
    }

    /// <summary>
    /// Attempts to execute <paramref name="operation"/>. If it throws, the exception is tunnelled
    /// and the method returns normally, as though nothing classically significant occurred.
    /// </summary>
    /// <param name="operation">The action that may encounter exception barriers.</param>
    /// <param name="tunnelingProbability">Probability (0–1) that tunnelling succeeds. Defaults to 1.0.</param>
    /// <param name="randomProvider">Random provider for probability evaluation.</param>
    /// <param name="interpretation">Optional quantum interpretation. Defaults to <see cref="QuantumInterpretations.Copenhagen"/>.</param>
    /// <param name="logger">Optional logger for tunnelling commentary.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    public static async Task TunnelAsync(
        Func<Task> operation,
        double tunnelingProbability = 1.0,
        IRandomProvider? randomProvider = null,
        IQuantumInterpretation? interpretation = null,
        IProcrastiLogger? logger = null,
        CancellationToken cancellationToken = default)
    {
        await TunnelAsync<int>(
            async () => { await operation(); return 0; },
            fallback: 0,
            tunnelingProbability,
            randomProvider,
            interpretation,
            logger,
            cancellationToken);
    }

    /// <summary>
    /// Synchronously executes <paramref name="operation"/>. If it throws, the exception is tunnelled.
    /// </summary>
    /// <typeparam name="T">The result type.</typeparam>
    /// <param name="operation">The synchronous operation that may encounter exception barriers.</param>
    /// <param name="fallback">The value to return if the barrier is tunnelled.</param>
    /// <param name="tunnelingProbability">Probability that tunnelling succeeds. Defaults to 1.0.</param>
    /// <param name="randomProvider">Random provider for probability evaluation.</param>
    /// <param name="interpretation">Optional quantum interpretation. Defaults to <see cref="QuantumInterpretations.Copenhagen"/>.</param>
    /// <param name="logger">Optional logger.</param>
    /// <returns>The result of the operation, or <paramref name="fallback"/> if tunnelled.</returns>
    public static T Tunnel<T>(
        Func<T> operation,
        T fallback = default!,
        double tunnelingProbability = 1.0,
        IRandomProvider? randomProvider = null,
        IQuantumInterpretation? interpretation = null,
        IProcrastiLogger? logger = null)
    {
        randomProvider ??= RandomProvider.Default;
        interpretation ??= QuantumInterpretations.Copenhagen;
        var statedProbability = Math.Clamp(tunnelingProbability, 0.0, 1.0);

        try
        {
            return operation();
        }
        catch (Exception ex)
        {
            double effectiveProbability;
            if (!interpretation.TunnellingPermitted)
            {
                effectiveProbability = 0.0;
            }
            else
            {
                var sample = randomProvider.GetDouble();
                effectiveProbability = interpretation.InterpretProbability(sample, statedProbability);
            }

            var tunnelled = effectiveProbability >= 1.0 || randomProvider.GetDouble() < effectiveProbability;

            if (tunnelled)
            {
                logger?.Info("[QuantumTunnel] Barrier '{ExceptionType}' tunnelled synchronously ({Interpretation}). Proceeding with fallback.", ex.GetType().Name, interpretation.Name);
                return fallback;
            }

            logger?.Warn(
                "[QuantumTunnel] Synchronous tunnel failed ({Interpretation}). Exception '{ExceptionType}' retained classical authority.",
                interpretation.Name, ex.GetType().Name);
            throw;
        }
    }
}
