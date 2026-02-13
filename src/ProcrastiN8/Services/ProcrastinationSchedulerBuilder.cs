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
    private IExecutionSafetyOptions? _safety;
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
    IProcrastinationSchedulerBuilder IProcrastinationSchedulerBuilder.AddMiddleware(IProcrastinationMiddleware middleware) => AddMiddleware(middleware);
    /// <summary>Overrides execution safety options (max cycles, etc.).</summary>
    public ProcrastinationSchedulerBuilderImpl WithSafety(IExecutionSafetyOptions safety) { _safety = safety; return this; }
    IProcrastinationSchedulerBuilder IProcrastinationSchedulerBuilder.WithSafety(IExecutionSafetyOptions safety) => WithSafety(safety);
    /// <summary>Convenience: attaches MetricsObserver (counters are emitted by default strategy instrumentation).</summary>
    public ProcrastinationSchedulerBuilderImpl WithMetrics() { _observers.Add(new Diagnostics.MetricsObserver()); return this; }
    IProcrastinationSchedulerBuilder IProcrastinationSchedulerBuilder.WithMetrics() { return WithMetrics(); }
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
        clone._safety = _safety;
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
        return ProcrastinationScheduler.Schedule(task, initialDelay, mode, _excuseProvider, _delayStrategy, _randomProvider, _timeProvider, ResolveFactory(), _observers, _middlewares, cancellationToken);
    }

    Task<ProcrastinationResult> IProcrastinationScheduler.ScheduleWithResult(Func<Task> task, TimeSpan initialDelay, ProcrastinationMode mode, CancellationToken cancellationToken)
    {
        EnsureBuilt();
        return ProcrastinationScheduler.ScheduleWithResult(task, initialDelay, mode, _excuseProvider, _delayStrategy, _randomProvider, _timeProvider, ResolveFactory(), _observers, _middlewares, cancellationToken);
    }

    ProcrastinationHandle IProcrastinationScheduler.ScheduleWithHandle(Func<Task> task, TimeSpan initialDelay, ProcrastinationMode mode, CancellationToken cancellationToken)
    {
        EnsureBuilt();
        return ProcrastinationScheduler.ScheduleWithHandle(task, initialDelay, mode, _excuseProvider, _delayStrategy, _randomProvider, _timeProvider, ResolveFactory(), _observers, _middlewares, cancellationToken);
    }

    private IProcrastinationStrategyFactory ResolveFactory()
    {
        var factory = _factory ?? new DefaultProcrastinationStrategyFactory();
        if (_safety is null)
        {
            return factory;
        }

        return new SafetyApplyingFactory(factory, _safety);
    }

    private sealed class SafetyApplyingFactory(IProcrastinationStrategyFactory inner, IExecutionSafetyOptions safety) : IProcrastinationStrategyFactory
    {
        public IProcrastinationStrategy Create(ProcrastinationMode mode)
        {
            var strategy = inner.Create(mode);
            if (strategy is ProcrastinationStrategyBase baseStrategy)
            {
                baseStrategy.ConfigureSafety(safety);
            }

            return strategy;
        }
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
