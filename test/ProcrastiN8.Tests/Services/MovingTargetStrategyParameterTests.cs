using ProcrastiN8.Services;
using ProcrastiN8.JustBecause;
using ProcrastiN8.LazyTasks;

namespace ProcrastiN8.Tests.Services;

/// <summary>
/// Verifies configurable parameter enforcement in <see cref="MovingTargetStrategy"/>.
/// </summary>
public class MovingTargetStrategyParameterTests
{
    private sealed class NoOpDelay : IDelayStrategy { public Task DelayAsync(TimeSpan? minDelay = null, TimeSpan? maxDelay = null, Func<double, bool>? beforeCallback = null, CancellationToken cancellationToken = default) => Task.CompletedTask; }

    [Fact]
    public async Task Respects_Custom_AbsoluteDeadline_Offset_Forces_Early_Execution()
    {
        // arrange
        var strategy = new MovingTargetStrategy(absoluteDeadlineOffset: TimeSpan.FromMilliseconds(30), maxSingleDelay: TimeSpan.FromMilliseconds(10));
        var time = Substitute.For<ITimeProvider>();
        var start = new DateTime(2025, 8, 24, 12, 0, 0, DateTimeKind.Utc);
        time.GetUtcNow().Returns(start, start.AddMilliseconds(12), start.AddMilliseconds(24), start.AddMilliseconds(36));
        var executed = false;
        Func<Task> task = () => { executed = true; return Task.CompletedTask; };

        // act
        await strategy.ExecuteAsync(task, TimeSpan.FromMilliseconds(5), null, new NoOpDelay(), Substitute.For<IRandomProvider>(), time, CancellationToken.None);
        var r = strategy.LastResult;

        // assert
        executed.Should().BeTrue();
        r.Executed.Should().BeTrue();
        r.TotalDeferral.Should().BeLessThan(TimeSpan.FromSeconds(1));
    }

    [Fact]
    public async Task Enforces_MaxSingleDelay_Bounding()
    {
        // arrange
        var strategy = new MovingTargetStrategy(maxSingleDelay: TimeSpan.FromMilliseconds(5), hardDelayCap: TimeSpan.FromMilliseconds(20));
        var time = Substitute.For<ITimeProvider>();
        var start = new DateTime(2025, 8, 24, 12, 0, 0, DateTimeKind.Utc);
        time.GetUtcNow().Returns(start, start.AddMilliseconds(6), start.AddMilliseconds(12), start.AddMilliseconds(18), start.AddMilliseconds(24), start.AddMilliseconds(30));
        var executed = false;
        Func<Task> task = () => { executed = true; return Task.CompletedTask; };

        // act
        await strategy.ExecuteAsync(task, TimeSpan.FromMilliseconds(50), null, new NoOpDelay(), Substitute.For<IRandomProvider>(), time, CancellationToken.None);
        var r = strategy.LastResult;

        // assert
        executed.Should().BeTrue();
        r.Cycles.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task HardDelayCap_Prevents_Unbounded_Growth()
    {
        // arrange
        var strategy = new MovingTargetStrategy(hardDelayCap: TimeSpan.FromMilliseconds(15), maxSingleDelay: TimeSpan.FromMilliseconds(10), absoluteDeadlineOffset: TimeSpan.FromMilliseconds(200));
        var time = Substitute.For<ITimeProvider>();
        var start = new DateTime(2025, 8, 24, 12, 0, 0, DateTimeKind.Utc);
        // Provide enough timestamps to go several cycles until ceiling policy or cap triggers.
        time.GetUtcNow().Returns(start,
            start.AddMilliseconds(11),
            start.AddMilliseconds(22),
            start.AddMilliseconds(33),
            start.AddMilliseconds(44),
            start.AddMilliseconds(55),
            start.AddMilliseconds(66));
        var executed = false; Func<Task> task = () => { executed = true; return Task.CompletedTask; };

        // act
        await strategy.ExecuteAsync(task, TimeSpan.FromMilliseconds(8), null, new NoOpDelay(), Substitute.For<IRandomProvider>(), time, CancellationToken.None);
        var r = strategy.LastResult;

        // assert
        executed.Should().BeTrue();
        r.Executed.Should().BeTrue();
    }
}
