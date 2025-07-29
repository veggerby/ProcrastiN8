using ProcrastiN8.LazyTasks;
using ProcrastiN8.JustBecause;

namespace ProcrastiN8.JustBecause.CollapseBehaviors;

public sealed class ReverseEntropyCollapseBehavior<T>(ITimeProvider? timeProvider = null, IRandomProvider? randomProvider = null) : ICollapseBehavior<T>
{
    private readonly ITimeProvider _timeProvider = timeProvider ?? new SystemTimeProvider();
    private readonly IRandomProvider _randomProvider = randomProvider ?? new RandomProvider();

    public async Task<T?> CollapseAsync(IEnumerable<IQuantumPromise<T>> entangled, CancellationToken cancellationToken)
    {
        var promises = entangled.ToArray();
        if (!promises.Any()) return default;

        foreach (var promise in promises)
        {
            var elapsedTime = _timeProvider.GetUtcNow() - promise.CreationTime;
            var successProbability = Math.Exp(-elapsedTime.TotalSeconds / 10); // Exponential decay

            await Task.Yield(); // Ensure asynchronous behavior

            if (_randomProvider.NextDouble() > successProbability)
            {
                throw new CollapseExpiredException("The promise has decayed into irrelevance.");
            }
        }

        return promises.First().Value;
    }
}

public class CollapseExpiredException : Exception
{
    public CollapseExpiredException(string message) : base(message) { }
}
