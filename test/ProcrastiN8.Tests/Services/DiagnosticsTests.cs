using ProcrastiN8.Services;
using ProcrastiN8.Services.Diagnostics;
using ProcrastiN8.Common;
using System.Diagnostics;

namespace ProcrastiN8.Tests.Services;

public class DiagnosticsTests
{
    private sealed class NoOpDelayStrategy : ProcrastiN8.LazyTasks.IDelayStrategy
    {
        public Task DelayAsync(TimeSpan? minDelay = null, TimeSpan? maxDelay = null, Func<double, bool>? beforeCallback = null, CancellationToken cancellationToken = default) => Task.CompletedTask;
    }
    [Fact]
    public async Task LoggingObserver_Emits_Log_Messages_For_Lifecycle()
    {
    var logger = Substitute.For<IProcrastiLogger>();
    var observer = new LoggingProcrastinationObserver(logger);
    var ctx = new ProcrastinationContext();
    await observer.OnCycleAsync(ctx);
    await observer.OnExcuseAsync(ctx);
    await observer.OnTriggeredAsync(ctx);
    await observer.OnAbandonedAsync(ctx);
    await observer.OnExecutedAsync(new ProcrastinationResult { Mode = ProcrastinationMode.MovingTarget, Cycles = 0, ExcuseCount = 0 });

    logger.Received().Debug(Arg.Is<string>(m => m.Contains("Cycle")));
    logger.Received().Info(Arg.Is<string>(m => m.Contains("Excuse")));
    logger.Received().Info(Arg.Is<string>(m => m.Contains("trigger", StringComparison.OrdinalIgnoreCase)));
    logger.Received().Warn(Arg.Is<string>(m => m.Contains("abandon", StringComparison.OrdinalIgnoreCase)));
    logger.Received().Info(Arg.Is<string>(m => m.Contains("Task executed")));
    }

    [Fact]
    public void StartActivity_Sets_Result_Tags()
    {
        var result = new ProcrastinationResult
        {
            Mode = ProcrastinationMode.WeekendFallback,
            Executed = true,
            Triggered = false,
            Abandoned = false,
            Cycles = 7,
            ExcuseCount = 4
        };
        using var listener = new ActivityListener
        {
            ShouldListenTo = s => s.Name == ProcrastinationDiagnostics.ActivitySourceName,
            Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllDataAndRecorded,
            SampleUsingParentId = (ref ActivityCreationOptions<string> _) => ActivitySamplingResult.AllDataAndRecorded
        };
        ActivitySource.AddActivityListener(listener);
        using var act = ProcrastinationDiagnostics.StartActivity("test-session", result);
    act.Should().NotBeNull();
    var mode = act!.GetTagItem("procrastination.mode")?.ToString();
    var cycles = act.GetTagItem("procrastination.cycles")?.ToString();
    var excuses = act.GetTagItem("procrastination.excuses")?.ToString();
    mode.Should().Be("WeekendFallback");
    cycles.Should().Be("7");
    excuses.Should().Be("4");
    }

    [Fact]
    public async Task InfiniteEstimation_Untriggered_Remains_Unexecuted()
    {
        using var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(200));
        var result = await ProcrastinationScheduler.ScheduleWithResult(
            () => Task.CompletedTask,
            TimeSpan.Zero,
            ProcrastinationMode.InfiniteEstimation,
            delayStrategy: Substitute.For<ProcrastiN8.LazyTasks.IDelayStrategy>(),
            randomProvider: Substitute.For<ProcrastiN8.JustBecause.IRandomProvider>(),
            cancellationToken: cts.Token);

        result.Executed.Should().BeFalse();
        result.ProductivityIndex.Should().Be(0.0);
    }
}
