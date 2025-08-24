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
    /// <summary>Finalizes the configuration and returns a scheduler instance.</summary>
    IProcrastinationScheduler Build();
}
