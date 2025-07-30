using ProcrastiN8.LazyTasks;

namespace ProcrastiN8.JustBecause.CollapseBehaviors;

public sealed class ReverseEntropyCollapseBehavior<T>(ITimeProvider? timeProvider = null, IRandomProvider? randomProvider = null, IProcrastiLogger? logger = null) : ICollapseBehavior<T>
{
    private readonly ITimeProvider _timeProvider = timeProvider ?? new SystemTimeProvider();
    private readonly IRandomProvider _randomProvider = randomProvider ?? RandomProvider.Default;
    private readonly IProcrastiLogger? _logger = logger;

    public async Task<T?> CollapseAsync(IEnumerable<IQuantumPromise<T>> entangled, CancellationToken cancellationToken)
    {
        var promises = entangled.ToArray();
        if (!promises.Any()) return default;

        foreach (var promise in promises)
        {
            var elapsedTime = _timeProvider.GetUtcNow() - promise.CreationTime;
            var successProbability = Math.Exp(-elapsedTime.TotalSeconds / 10); // Exponential decay

            _logger?.Info($"Evaluating promise with elapsed time: {elapsedTime.TotalSeconds}s and success probability: {successProbability}");

            await Task.Yield(); // Ensure asynchronous behavior

            if (_randomProvider.GetDouble() > successProbability)
            {
                _logger?.Error("Promise decayed into irrelevance.");
                throw new CollapseExpiredException("The promise has decayed into irrelevance.");
            }
        }

        return promises.First().Value;
    }
}

public class CollapseExpiredException(string message) : Exception(message)
{
}