using ProcrastiN8.Common;
using ProcrastiN8.JustBecause;
using ProcrastiN8.LazyTasks;
using ProcrastiN8.Services;

namespace ProcrastiN8.Tests.Services;

public class ProcrastinationObserverTests
{
    private sealed class CountingObserver : IProcrastinationObserver
    {
        public int Cycles; public int Excuses; public bool Executed;
        public Task OnCycleAsync(ProcrastinationContext context, CancellationToken cancellationToken = default) { Cycles++; return Task.CompletedTask; }
        public Task OnExcuseAsync(ProcrastinationContext context, CancellationToken cancellationToken = default) { Excuses = context.ExcuseCount; return Task.CompletedTask; }
        public Task OnExecutedAsync(ProcrastinationResult result, CancellationToken cancellationToken = default) { Executed = result.Executed; return Task.CompletedTask; }
    }

    [Fact]
    public async Task Observer_Is_Invoked_For_MovingTarget()
    {
        // arrange
        var observer = new CountingObserver();
        Func<Task> task = () => Task.CompletedTask;
        var delayStrategy = Substitute.For<IDelayStrategy>();
        delayStrategy.DelayAsync(Arg.Any<TimeSpan>(), Arg.Any<TimeSpan?>(), Arg.Any<Func<double, bool>>(), Arg.Any<CancellationToken>())
            .Returns(async _ => await Task.Delay(1));
        var randomProvider = Substitute.For<IRandomProvider>();
        randomProvider.GetDouble().Returns(0.1);

        // act
        var result = await ProcrastinationScheduler.ScheduleWithResult(
            task,
            TimeSpan.FromMilliseconds(5),
            ProcrastinationMode.MovingTarget,
            observers: new[] { observer },
            delayStrategy: delayStrategy,
            randomProvider: randomProvider);

        // assert
        result.Executed.Should().BeTrue();
        observer.Executed.Should().BeTrue();
        observer.Cycles.Should().BeGreaterThan(0);
    }
}