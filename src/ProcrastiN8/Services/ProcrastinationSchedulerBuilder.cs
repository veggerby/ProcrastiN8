using ProcrastiN8.Common;
using ProcrastiN8.JustBecause;
using ProcrastiN8.LazyTasks;

namespace ProcrastiN8.Services;

internal sealed class ProcrastinationSchedulerBuilderImpl : IProcrastinationSchedulerBuilder, IProcrastinationScheduler
{
    private IExcuseProvider? _excuseProvider;
    private IDelayStrategy? _delayStrategy;
    private IRandomProvider? _randomProvider;
    private ITimeProvider? _timeProvider;
    private IProcrastinationStrategyFactory? _factory;
    private readonly List<IProcrastinationObserver> _observers = new();
    private readonly List<IProcrastinationMiddleware> _middlewares = new();
    private bool _built;

    public static IProcrastinationSchedulerBuilder Create() => new ProcrastinationSchedulerBuilderImpl();

    /// <inheritdoc />
    public IProcrastinationSchedulerBuilder WithExcuseProvider(IExcuseProvider provider) { _excuseProvider = provider; return this; }
    /// <inheritdoc />
    public IProcrastinationSchedulerBuilder WithDelayStrategy(IDelayStrategy strategy) { _delayStrategy = strategy; return this; }
    /// <inheritdoc />
    public IProcrastinationSchedulerBuilder WithRandomProvider(IRandomProvider provider) { _randomProvider = provider; return this; }
    /// <inheritdoc />
    public IProcrastinationSchedulerBuilder WithTimeProvider(ITimeProvider provider) { _timeProvider = provider; return this; }
    /// <inheritdoc />
    public IProcrastinationSchedulerBuilder WithFactory(IProcrastinationStrategyFactory factory) { _factory = factory; return this; }
    /// <inheritdoc />
    public IProcrastinationSchedulerBuilder AddObserver(IProcrastinationObserver observer) { _observers.Add(observer); return this; }
    /// <summary>Adds a middleware component.</summary>
    public ProcrastinationSchedulerBuilderImpl AddMiddleware(IProcrastinationMiddleware middleware) { _middlewares.Add(middleware); return this; }
    /// <summary>Resets previously configured components (observers & middlewares preserved unless reset called).</summary>
    public ProcrastinationSchedulerBuilderImpl Reset(bool preserveObservers = true)
    {
        _excuseProvider = null; _delayStrategy = null; _randomProvider = null; _timeProvider = null; _factory = null;
        if (!preserveObservers) { _observers.Clear(); }
        _middlewares.Clear();
        return this;
    }
    /// <summary>Creates a shallow clone (observers & middleware lists copied).</summary>
    public ProcrastinationSchedulerBuilderImpl Clone()
    {
        var clone = new ProcrastinationSchedulerBuilderImpl();
        clone._excuseProvider = _excuseProvider;
        clone._delayStrategy = _delayStrategy;
        clone._randomProvider = _randomProvider;
        clone._timeProvider = _timeProvider;
        clone._factory = _factory;
        foreach (var o in _observers) clone._observers.Add(o);
        foreach (var m in _middlewares) clone._middlewares.Add(m);
        return clone;
    }

    /// <inheritdoc />
    public IProcrastinationScheduler Build()
    {
        _built = true;
        return this;
    }

    private void EnsureBuilt()
    {
        if (!_built)
        {
            throw new InvalidOperationException("Builder not finalized. Call Build() before using the scheduler.");
        }
    }

    Task IProcrastinationScheduler.Schedule(Func<Task> task, TimeSpan initialDelay, ProcrastinationMode mode, CancellationToken cancellationToken)
    {
        EnsureBuilt();
    return ProcrastinationScheduler.Schedule(task, initialDelay, mode, _excuseProvider, _delayStrategy, _randomProvider, _timeProvider, cancellationToken);
    }

    Task<ProcrastinationResult> IProcrastinationScheduler.ScheduleWithResult(Func<Task> task, TimeSpan initialDelay, ProcrastinationMode mode, CancellationToken cancellationToken)
    {
        EnsureBuilt();
    return ProcrastinationScheduler.ScheduleWithResult(task, initialDelay, mode, _excuseProvider, _delayStrategy, _randomProvider, _timeProvider, _factory, _observers, cancellationToken);
    }

    ProcrastinationHandle IProcrastinationScheduler.ScheduleWithHandle(Func<Task> task, TimeSpan initialDelay, ProcrastinationMode mode, CancellationToken cancellationToken)
    {
        EnsureBuilt();
    return ProcrastinationScheduler.ScheduleWithHandle(task, initialDelay, mode, _excuseProvider, _delayStrategy, _randomProvider, _timeProvider, _factory, _observers, cancellationToken);
    }
}

/// <summary>
/// Entry point for creating a procrastination scheduler builder.
/// </summary>
public static class ProcrastinationSchedulerBuilder
{
    /// <summary>Creates a new builder instance.</summary>
    public static IProcrastinationSchedulerBuilder Create() => ProcrastinationSchedulerBuilderImpl.Create();
    /// <summary>Convenience for adding logging observer.</summary>
    public static IProcrastinationSchedulerBuilder CreateWithLogging(IProcrastiLogger logger) => Create().AddObserver(new LoggingProcrastinationObserver(logger));
}
