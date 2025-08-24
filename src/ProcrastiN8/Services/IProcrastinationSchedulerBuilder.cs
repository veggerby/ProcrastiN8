using ProcrastiN8.Common;
using ProcrastiN8.JustBecause;
using ProcrastiN8.LazyTasks;

namespace ProcrastiN8.Services;

/// <summary>
/// Fluent builder for constructing an <see cref="IProcrastinationScheduler"/> with custom providers and observers.
/// </summary>
public interface IProcrastinationSchedulerBuilder
{
    /// <summary>Registers a custom excuse provider used to source ceremonial rationalizations.</summary>
    IProcrastinationSchedulerBuilder WithExcuseProvider(IExcuseProvider provider);
    /// <summary>Registers a delay strategy controlling temporal pacing.</summary>
    IProcrastinationSchedulerBuilder WithDelayStrategy(IDelayStrategy strategy);
    /// <summary>Registers the pseudo-random source employed by strategies.</summary>
    IProcrastinationSchedulerBuilder WithRandomProvider(IRandomProvider provider);
    /// <summary>Registers a time provider to enable deterministic time travel in tests.</summary>
    IProcrastinationSchedulerBuilder WithTimeProvider(ITimeProvider provider);
    /// <summary>Registers a strategy factory enabling custom procrastination behaviors.</summary>
    IProcrastinationSchedulerBuilder WithFactory(IProcrastinationStrategyFactory factory);
    /// <summary>Adds an observer to receive lifecycle instrumentation callbacks.</summary>
    IProcrastinationSchedulerBuilder AddObserver(IProcrastinationObserver observer);
    /// <summary>Adds a middleware component that can wrap execution for cross-cutting concerns (metrics, logging, chaos).</summary>
    IProcrastinationSchedulerBuilder AddMiddleware(IProcrastinationMiddleware middleware);
    /// <summary>
    /// Sets ambient execution safety options (e.g., MaxCycles) adopted by strategies that have not explicitly configured safety.
    /// This is a process-wide override until replaced; intended for test harnesses or global configuration.
    /// </summary>
    IProcrastinationSchedulerBuilder WithSafety(IExecutionSafetyOptions safety);
    /// <summary>
    /// Attaches a metrics observer translating structured lifecycle events into exported counters.
    /// Optional: counters are also emitted automatically; use this when you need the observer event stream for custom sinks.
    /// </summary>
    IProcrastinationSchedulerBuilder WithMetrics();
    /// <summary>Finalizes the configuration and returns a scheduler instance.</summary>
    IProcrastinationScheduler Build();
}
