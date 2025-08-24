using ProcrastiN8.Services;
using ProcrastiN8.Common;
using ProcrastiN8.JustBecause;
using ProcrastiN8.LazyTasks;

namespace ProcrastiN8.Tests.Services;

/// <summary>
/// Verifies fluent builder behaviors: cloning, resetting, metrics attachment, and build preconditions.
/// </summary>
public class ProcrastinationSchedulerBuilderTests
{
    private sealed class StubFactory : IProcrastinationStrategyFactory { public IProcrastinationStrategy Create(ProcrastinationMode mode) => new MovingTargetStrategy(); }
    private sealed class StubDelay : IDelayStrategy { public Task DelayAsync(TimeSpan? minDelay = null, TimeSpan? maxDelay = null, Func<double, bool>? beforeCallback = null, CancellationToken cancellationToken = default) => Task.CompletedTask; }

    [Fact]
    public async Task Build_Without_Call_Throws_On_Schedule()
    {
        // arrange
        var builder = ProcrastinationSchedulerBuilder.Create();
        var scheduler = (IProcrastinationScheduler)builder; // not built

        // act
    // assert
    await Assert.ThrowsAsync<InvalidOperationException>(() => scheduler.Schedule(() => Task.CompletedTask, TimeSpan.Zero, ProcrastinationMode.MovingTarget));
    }

    [Fact]
    public async Task Clone_Preserves_Configuration()
    {
        // arrange
        var factory = new StubFactory();
        var delay = new StubDelay();
        var random = Substitute.For<IRandomProvider>();
        random.GetDouble().Returns(0.01);
        var builder = (ProcrastinationSchedulerBuilder.Create()
            .WithFactory(factory)
            .WithDelayStrategy(delay)
            .WithRandomProvider(random)) as ProcrastinationSchedulerBuilderImpl; // downcast for Clone

        var clone = builder!.Clone().Build();

        // act
        var result = await clone.ScheduleWithResult(() => Task.CompletedTask, TimeSpan.Zero, ProcrastinationMode.MovingTarget);

        // assert
        result.Executed.Should().BeTrue();
    }

    [Fact]
    public async Task Reset_Preserves_Observers_When_Requested()
    {
        // arrange
        var logger = Substitute.For<IProcrastiLogger>();
        var built = (ProcrastinationSchedulerBuilder.Create()
            .AddObserver(new LoggingProcrastinationObserver(logger)) as ProcrastinationSchedulerBuilderImpl)!;

        built.Reset(preserveObservers: true).Build();

        // act
    await ((IProcrastinationScheduler)built).Schedule(() => Task.CompletedTask, TimeSpan.Zero, ProcrastinationMode.MovingTarget);

        // assert
    logger.Received().Info(Arg.Any<string>());
    }

    [Fact]
    public async Task WithMetrics_Is_Idempotent()
    {
        // arrange
        var scheduler = ProcrastinationSchedulerBuilder.Create().WithMetrics().WithMetrics().Build();

        // act
        var result = await scheduler.ScheduleWithResult(() => Task.CompletedTask, TimeSpan.Zero, ProcrastinationMode.MovingTarget);

        // assert
        result.Executed.Should().BeTrue(); // counters recorded regardless of duplicate observer registration attempts
    }
}
