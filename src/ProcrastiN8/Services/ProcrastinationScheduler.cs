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
        IProcrastinationStrategyFactory? factory = null,
        IEnumerable<IProcrastinationObserver>? observers = null,
        IEnumerable<IProcrastinationMiddleware>? middlewares = null,
        CancellationToken cancellationToken = default)
    {
        await ScheduleInternal(task, initialDelay, mode, excuseProvider, delayStrategy, randomProvider, timeProvider, factory, observers, middlewares, cancellationToken);
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
        IEnumerable<IProcrastinationObserver>? observers = null,
        IEnumerable<IProcrastinationMiddleware>? middlewares = null,
        CancellationToken cancellationToken = default) =>
        await ScheduleInternal(task, initialDelay, mode, excuseProvider, delayStrategy, randomProvider, timeProvider, factory, observers, middlewares, cancellationToken);

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
        IEnumerable<IProcrastinationMiddleware>? middlewares = null,
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
        var correlationId = Guid.NewGuid();
        var execContext = new ProcrastinationExecutionContext(mode, correlationId);
        if (strategy is IResultReportingProcrastinationStrategy rrPre)
        {
            rrPre.LastResult.CorrelationId = correlationId;
        }
    Task FinalExecute() => strategy.ExecuteAsync(task, initialDelay, excuseProvider, delayStrategy, randomProvider, timeProvider, cancellationToken);
    var pipeline = BuildMiddlewarePipeline(FinalExecute, middlewares, execContext, strategy, cancellationToken);
    await pipeline();
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
        IEnumerable<IProcrastinationMiddleware>? middlewares = null,
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
                var correlationId = Guid.NewGuid();
                var execContext = new ProcrastinationExecutionContext(mode, correlationId);
                if (strategy is IResultReportingProcrastinationStrategy rrPre)
                {
                    rrPre.LastResult.CorrelationId = correlationId;
                }
                Task FinalExecute() => strategy.ExecuteAsync(task, initialDelay, excuseProvider, delayStrategy, randomProvider, timeProvider, cancellationToken);
                var pipeline = BuildMiddlewarePipeline(FinalExecute, middlewares, execContext, strategy, cancellationToken);
                await pipeline();
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
        IEnumerable<IProcrastinationObserver>? observers,
        IEnumerable<IProcrastinationMiddleware>? middlewares,
        CancellationToken cancellationToken)
    {
        delayStrategy ??= new DefaultDelayStrategy();
        randomProvider ??= RandomProvider.Default;
        timeProvider ??= SystemTimeProvider.Default;
        factory ??= new DefaultProcrastinationStrategyFactory();

        var strategy = factory.Create(mode);
        if (strategy is ProcrastinationStrategyBase baseStrategy)
        {
            baseStrategy.AttachObservers(observers);
        }
        var correlationId = Guid.NewGuid();
        var execContext = new ProcrastinationExecutionContext(mode, correlationId);
        if (strategy is IResultReportingProcrastinationStrategy rrPre)
        {
            rrPre.LastResult.CorrelationId = correlationId;
        }
        Task FinalExecute() => strategy.ExecuteAsync(task, initialDelay, excuseProvider, delayStrategy, randomProvider, timeProvider, cancellationToken);
        var pipeline = BuildMiddlewarePipeline(FinalExecute, middlewares, execContext, strategy, cancellationToken);
        await pipeline();
    }

    private static Func<Task> BuildMiddlewarePipeline(
        Func<Task> terminal,
        IEnumerable<IProcrastinationMiddleware>? middlewares,
        ProcrastinationExecutionContext context,
        IProcrastinationStrategy strategy,
        CancellationToken cancellationToken)
    {
        // Wrap terminal to capture result before unwinding so 'after' sections observe execution outcome.
        Func<Task> next = async () =>
        {
            await terminal();
            if (strategy is IResultReportingProcrastinationStrategy rr)
            {
                context.Result = rr.LastResult;
            }
        };
        if (middlewares == null)
        {
            return next;
        }
        var ordered = middlewares.Reverse().ToArray();
        foreach (var m in ordered)
        {
            var localNext = next;
            next = () => m.InvokeAsync(context, localNext, cancellationToken);
        }
        return next;
    }
}
