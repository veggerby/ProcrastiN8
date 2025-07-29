using ProcrastiN8.LazyTasks;

namespace ProcrastiN8.JustBecause;

/// <summary>
/// Enables the creation of pointless, never-ending chains of asynchronous operations.
/// </summary>
/// <remarks>
/// This class is intentionally over-engineered to simulate the appearance of productivity.
/// </remarks>
/// <remarks>
/// Initializes a new instance of the <see cref="PointlessChain"/> class.
/// </remarks>
/// <param name="delayStrategy">An optional delay strategy for arbitrary stalling.</param>
/// <param name="logger">An optional logger for chain commentary.</param>
public class PointlessChain(IDelayStrategy? delayStrategy = null, IProcrastiLogger? logger = null)
{
    private readonly IDelayStrategy? _delayStrategy = delayStrategy;
    private readonly IProcrastiLogger? _logger = logger;

    /// <summary>
    /// Begins a chain of asynchronous operations that never completes.
    /// </summary>
    /// <param name="steps">The number of steps to simulate before looping back.</param>
    /// <param name="cancellationToken">A token to cancel the chain.</param>
    public async Task StartAsync(int steps = 5, CancellationToken cancellationToken = default)
    {
        if (steps < 1) steps = 1;
        var current = 0;
        while (!cancellationToken.IsCancellationRequested)
        {
            _logger?.Info($"PointlessChain step {current + 1} of {steps} — still going nowhere.");
            if (_delayStrategy != null)
            {
                await _delayStrategy.DelayAsync(cancellationToken: cancellationToken);
            }

            if (cancellationToken.IsCancellationRequested)
            {
                _logger?.Info("Cancellation requested. Exiting loop.");
                break;
            }

            current = (current + 1) % steps;
        }
    }
}