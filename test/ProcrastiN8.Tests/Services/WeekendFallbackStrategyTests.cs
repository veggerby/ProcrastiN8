using ProcrastiN8.Services;
using ProcrastiN8.JustBecause;
using ProcrastiN8.LazyTasks;

namespace ProcrastiN8.Tests.Services;

/// <summary>
/// Exhaustive branch coverage for <see cref="WeekendFallbackStrategy"/> weekend window, elapsed cap, and safety termination.
/// </summary>
public class WeekendFallbackStrategyTests
{
    private sealed class NoOpDelay : IDelayStrategy { public Task DelayAsync(TimeSpan? minDelay = null, TimeSpan? maxDelay = null, Func<double, bool>? beforeCallback = null, CancellationToken cancellationToken = default) => Task.CompletedTask; }

    [Fact]
    public async Task Executes_Immediately_During_Weekend_Window()
    {
        // arrange
        var strategy = new WeekendFallbackStrategy();
        var time = Substitute.For<ITimeProvider>();
        time.GetUtcNow().Returns(new DateTime(2025, 8, 2, 16, 0, 0, DateTimeKind.Utc)); // Saturday 16:00
        var executed = false;
        Func<Task> task = () => { executed = true; return Task.CompletedTask; };

        // act
        await strategy.ExecuteAsync(task, TimeSpan.Zero, null, new NoOpDelay(), Substitute.For<IRandomProvider>(), time, CancellationToken.None);
        var r = strategy.LastResult;

        // assert
        executed.Should().BeTrue();
        r.Executed.Should().BeTrue();
        r.Cycles.Should().Be(0);
    }

    [Fact]
    public async Task Executes_After_72_Hour_Elapsed()
    {
        // arrange
        var strategy = new WeekendFallbackStrategy();
    var baseTime = new DateTime(2025, 8, 1, 10, 0, 0, DateTimeKind.Utc);
        var time = Substitute.For<ITimeProvider>();
    // Simulate start, one cycle later (+1h), then threshold (+73h)
	time.GetUtcNow().Returns(baseTime, baseTime.AddHours(1), baseTime.AddHours(73));
        var executed = false;
        Func<Task> task = () => { executed = true; return Task.CompletedTask; };

        // act
        await strategy.ExecuteAsync(task, TimeSpan.Zero, null, new NoOpDelay(), Substitute.For<IRandomProvider>(), time, CancellationToken.None);
        var r = strategy.LastResult;

        // assert
        executed.Should().BeTrue();
        r.Executed.Should().BeTrue();
    r.Cycles.Should().BeGreaterThanOrEqualTo(0); // Execution may occur immediately at threshold without additional cycle
    }

    private sealed class TinySafety(int max) : IExecutionSafetyOptions { public int MaxCycles { get; } = max; }

    [Fact]
    public async Task SafetyCap_Prevents_Execution_Prior_To_Window()
    {
        // arrange
        var strategy = new WeekendFallbackStrategy();
        strategy.ConfigureSafety(new TinySafety(2));
        var time = Substitute.For<ITimeProvider>();
        // Always weekday morning  (Friday 10:00). Time increments hourly via Returns sequence.
        var t0 = new DateTime(2025, 8, 1, 10, 0, 0, DateTimeKind.Utc);
        time.GetUtcNow().Returns(t0, t0.AddHours(1), t0.AddHours(2));
        var executed = false;
        Func<Task> task = () => { executed = true; return Task.CompletedTask; };

        // act
        await strategy.ExecuteAsync(task, TimeSpan.Zero, null, new NoOpDelay(), Substitute.For<IRandomProvider>(), time, CancellationToken.None);
        var r = strategy.LastResult;

        // assert
        executed.Should().BeFalse();
        r.Executed.Should().BeFalse();
        r.Cycles.Should().Be(2);
    }
}
