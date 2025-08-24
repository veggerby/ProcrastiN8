using ProcrastiN8.Common;
using ProcrastiN8.JustBecause;
using ProcrastiN8.LazyTasks;

namespace ProcrastiN8.Services;

public static class ProcrastinationScheduler
{
    public static async Task Schedule(
        Func<Task> task,
        TimeSpan initialDelay,
        ProcrastinationMode mode,
        IExcuseProvider? excuseProvider = null,
        IDelayStrategy? delayStrategy = null,
        IRandomProvider? randomProvider = null,
        ITimeProvider? timeProvider = null,
        CancellationToken cancellationToken = default)
    {
        await ScheduleInternal(task, initialDelay, mode, excuseProvider, delayStrategy, randomProvider, timeProvider, factory: null, cancellationToken);
    }

    /// <summary>
    /// Overload allowing a caller-supplied factory.
    /// </summary>
    public static async Task Schedule(
        Func<Task> task,
        TimeSpan initialDelay,
        ProcrastinationMode mode,
        IProcrastinationStrategyFactory factory,
        IExcuseProvider? excuseProvider = null,
        IDelayStrategy? delayStrategy = null,
        IRandomProvider? randomProvider = null,
        ITimeProvider? timeProvider = null,
        CancellationToken cancellationToken = default) =>
        await ScheduleInternal(task, initialDelay, mode, excuseProvider, delayStrategy, randomProvider, timeProvider, factory, cancellationToken);

    /// <summary>
    /// Schedules and returns a detailed result.
    /// </summary>
    public static async Task<ProcrastinationResult> ScheduleWithResult(
        Func<Task> task,
        TimeSpan initialDelay,
        ProcrastinationMode mode,
        IExcuseProvider? excuseProvider = null,
        IDelayStrategy? delayStrategy = null,
        IRandomProvider? randomProvider = null,
        ITimeProvider? timeProvider = null,
        IProcrastinationStrategyFactory? factory = null,
        IEnumerable<IProcrastinationObserver>? observers = null,
        CancellationToken cancellationToken = default)
    {
        delayStrategy ??= new DefaultDelayStrategy();
        randomProvider ??= RandomProvider.Default;
        timeProvider ??= SystemTimeProvider.Default;
        factory ??= new DefaultProcrastinationStrategyFactory();

        var strategy = factory.Create(mode);
        ProcrastinationHandle? handle = null;
        if (strategy is ProcrastinationStrategyBase baseStrategy)
        {
            handle = new ProcrastinationHandle(mode);
            baseStrategy.AttachControl(handle);
            baseStrategy.AttachObservers(observers);
        }
    await strategy.ExecuteAsync(task, initialDelay, excuseProvider, delayStrategy, randomProvider, timeProvider, cancellationToken);
        if (strategy is IResultReportingProcrastinationStrategy reporting)
        {
            var r = reporting.LastResult;
            r.Mode = mode;
            handle?.Complete(r);
            return r;
        }

    return new ProcrastinationResult { Mode = mode, Executed = true, TotalDeferral = TimeSpan.Zero, ExcuseCount = 0, Cycles = 0 };
    }

    /// <summary>
    /// Schedules and returns a handle that can interact with the workflow.
    /// </summary>
    public static ProcrastinationHandle ScheduleWithHandle(
        Func<Task> task,
        TimeSpan initialDelay,
        ProcrastinationMode mode,
        IExcuseProvider? excuseProvider = null,
        IDelayStrategy? delayStrategy = null,
        IRandomProvider? randomProvider = null,
        ITimeProvider? timeProvider = null,
        IProcrastinationStrategyFactory? factory = null,
        IEnumerable<IProcrastinationObserver>? observers = null,
        CancellationToken cancellationToken = default)
    {
        delayStrategy ??= new DefaultDelayStrategy();
        randomProvider ??= RandomProvider.Default;
        timeProvider ??= SystemTimeProvider.Default;
        factory ??= new DefaultProcrastinationStrategyFactory();

        var strategy = factory.Create(mode);
        var handle = new ProcrastinationHandle(mode);
        if (strategy is ProcrastinationStrategyBase baseStrategy)
        {
            baseStrategy.AttachControl(handle);
            baseStrategy.AttachObservers(observers);
        }

        _ = Task.Run(async () =>
        {
            try
            {
                await strategy.ExecuteAsync(task, initialDelay, excuseProvider, delayStrategy, randomProvider, timeProvider, cancellationToken);
                if (strategy is IResultReportingProcrastinationStrategy reporting)
                {
                    var r = reporting.LastResult;
                    r.Mode = mode;
                    handle.Complete(r);
                }
                else
                {
                    handle.Complete(new ProcrastinationResult { Mode = mode, Executed = true });
                }
            }
            catch (OperationCanceledException)
            {
                handle.Complete(new ProcrastinationResult { Mode = mode, Executed = false });
            }
            catch
            {
                handle.Cancel();
            }
        }, CancellationToken.None);

        return handle;
    }

    private static async Task ScheduleInternal(
        Func<Task> task,
        TimeSpan initialDelay,
        ProcrastinationMode mode,
        IExcuseProvider? excuseProvider,
        IDelayStrategy? delayStrategy,
        IRandomProvider? randomProvider,
        ITimeProvider? timeProvider,
        IProcrastinationStrategyFactory? factory,
        CancellationToken cancellationToken)
    {
        delayStrategy ??= new DefaultDelayStrategy();
        randomProvider ??= RandomProvider.Default;
        timeProvider ??= SystemTimeProvider.Default;
        factory ??= new DefaultProcrastinationStrategyFactory();

        var strategy = factory.Create(mode);
        await strategy.ExecuteAsync(task, initialDelay, excuseProvider, delayStrategy, randomProvider, timeProvider, cancellationToken);
    }
}
