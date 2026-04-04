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
    /// <param name="logger">Optional logger for tunnelling event commentary.</param>
    /// <param name="cancellationToken">Token to cancel the operation before tunnelling is needed.</param>
    /// <returns>The result of the operation, or <paramref name="fallback"/> if the barrier was tunnelled.</returns>
    public static async Task<T> TunnelAsync<T>(
        Func<Task<T>> operation,
        T fallback = default!,
        double tunnelingProbability = 1.0,
        IRandomProvider? randomProvider = null,
        IProcrastiLogger? logger = null,
        CancellationToken cancellationToken = default)
    {
        randomProvider ??= RandomProvider.Default;
        tunnelingProbability = Math.Clamp(tunnelingProbability, 0.0, 1.0);

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
            var tunnelled = randomProvider.GetDouble() < tunnelingProbability;

            if (tunnelled)
            {
                logger?.Info(
                    "[QuantumTunnel] Barrier '{ExceptionType}' encountered and tunnelled. Proceeding with fallback value. Classical physics need not apply.",
                    ex.GetType().Name);
                return fallback;
            }

            logger?.Warn(
                "[QuantumTunnel] Tunnelling failed ({Probability:P0} probability). The exception '{ExceptionType}' retains classical authority in this timeline.",
                tunnelingProbability, ex.GetType().Name);
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
    /// <param name="logger">Optional logger for tunnelling commentary.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    public static async Task TunnelAsync(
        Func<Task> operation,
        double tunnelingProbability = 1.0,
        IRandomProvider? randomProvider = null,
        IProcrastiLogger? logger = null,
        CancellationToken cancellationToken = default)
    {
        await TunnelAsync<int>(
            async () => { await operation(); return 0; },
            fallback: 0,
            tunnelingProbability,
            randomProvider,
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
    /// <param name="logger">Optional logger.</param>
    /// <returns>The result of the operation, or <paramref name="fallback"/> if tunnelled.</returns>
    public static T Tunnel<T>(
        Func<T> operation,
        T fallback = default!,
        double tunnelingProbability = 1.0,
        IRandomProvider? randomProvider = null,
        IProcrastiLogger? logger = null)
    {
        randomProvider ??= RandomProvider.Default;
        tunnelingProbability = Math.Clamp(tunnelingProbability, 0.0, 1.0);

        try
        {
            return operation();
        }
        catch (Exception ex)
        {
            var tunnelled = randomProvider.GetDouble() < tunnelingProbability;

            if (tunnelled)
            {
                logger?.Info("[QuantumTunnel] Barrier '{ExceptionType}' tunnelled synchronously. Proceeding with fallback.", ex.GetType().Name);
                return fallback;
            }

            logger?.Warn(
                "[QuantumTunnel] Synchronous tunnel failed. Exception '{ExceptionType}' retained classical authority.",
                ex.GetType().Name);
            throw;
        }
    }
}
