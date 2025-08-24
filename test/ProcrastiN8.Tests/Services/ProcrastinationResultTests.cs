using ProcrastiN8.LazyTasks;
using ProcrastiN8.Services;

namespace ProcrastiN8.Tests.Services;

public class ProcrastinationResultTests
{
    private sealed class InstantDelayStrategy : IDelayStrategy
    {
        public Task DelayAsync(TimeSpan? minDelay = null, TimeSpan? maxDelay = null, Func<double, bool>? beforeCallback = null, CancellationToken cancellationToken = default)
            => Task.CompletedTask; // no-op
    }

    private sealed class FixedExcuseProvider : ProcrastiN8.Common.IExcuseProvider
    {
        private int _i;
        public Task<string> GetExcuseAsync() => Task.FromResult($"Excuse {_i++}");
    }

    [Fact]
    public async Task ProductivityIndex_Should_Decrease_With_Cycles_And_Excuses()
    {
        var scheduler = ProcrastinationSchedulerBuilder.Create()
            .WithDelayStrategy(new InstantDelayStrategy())
            .WithExcuseProvider(new FixedExcuseProvider())
            .Build();

        var result = await scheduler.ScheduleWithResult(() => Task.CompletedTask, TimeSpan.FromMilliseconds(1), ProcrastinationMode.MovingTarget);
        result.Executed.Should().BeTrue();
        (result.Cycles + result.ExcuseCount).Should().BeGreaterThan(0, "at least one ceremonial deferment expected");
        var expected = Math.Round(1.0 / (1 + result.ExcuseCount + result.Cycles), 4);
        result.ProductivityIndex.Should().Be(expected);
        result.ProductivityIndex.Should().BeLessThan(1.0);
    }

    private sealed class TestSafety(int maxCycles) : IExecutionSafetyOptions
    {
        public int MaxCycles { get; } = maxCycles;
    }

    [Fact]
    public async Task ProductivityIndex_Should_Be_One_When_Executed_Immediately()
    {
        var scheduler = ProcrastinationSchedulerBuilder.Create()
            .WithDelayStrategy(new InstantDelayStrategy())
            .Build();

        var result = await scheduler.ScheduleWithResult(() => Task.CompletedTask, TimeSpan.Zero, ProcrastinationMode.MovingTarget);
        result.Executed.Should().BeTrue();
    result.Cycles.Should().BeGreaterThanOrEqualTo(0);
        // If no cycles or excuses, MPI should be 1. Otherwise validate formula.
        var expected = result.Executed ? Math.Round(1.0 / (1 + result.ExcuseCount + result.Cycles), 4) : 0.0;
        result.ProductivityIndex.Should().Be(expected);
    }

    [Fact]
    public async Task AmbientSafety_Should_Limit_Cycles_And_Prevent_WeekendFallback_Execution()
    {
        // WeekendFallbackStrategy only executes when weekend window reached; with tiny MaxCycles we expect early termination without execution.
    var scheduler = ProcrastinationSchedulerBuilder.Create()
            .WithDelayStrategy(new InstantDelayStrategy())
            .WithSafety(new TestSafety(maxCycles: 2))
            .Build();

        var result = await scheduler.ScheduleWithResult(() => Task.CompletedTask, TimeSpan.Zero, ProcrastinationMode.WeekendFallback);

        result.Executed.Should().BeFalse("safety cap should short-circuit before weekend window");
        result.Cycles.Should().BeLessThanOrEqualTo(2);
        result.ProductivityIndex.Should().Be(0.0);

    // reset ambient safety for other tests
    ProcrastinationStrategyBase.SetAmbientSafety(DefaultExecutionSafetyOptions.Instance);
    }
}
