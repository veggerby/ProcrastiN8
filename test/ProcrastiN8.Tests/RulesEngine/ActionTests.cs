using ProcrastiN8.JustBecause;
using ProcrastiN8.RulesEngine;
using ProcrastiN8.RulesEngine.Actions;

namespace ProcrastiN8.Tests.RulesEngine;

public class ActionTests
{
    [Fact]
    public async Task FixedDeferralAction_ReturnsDeferralDuration()
    {
        // arrange
        var action = new FixedDeferralAction(TimeSpan.FromMinutes(30));
        var task = new ProcrastinationTask();
        var randomProvider = Substitute.For<IRandomProvider>();
        randomProvider.GetDouble().Returns(0.5);
        var context = new RuleEvaluationContext(task, randomProvider: randomProvider);

        // act
        var result = await action.ExecuteAsync(context, CancellationToken.None);

        // assert
        result.DeferralDuration.Should().Be(TimeSpan.FromMinutes(30), "fixed deferral should return the specified duration");
        result.Excuse.Should().NotBeNullOrEmpty("even fixed deferrals deserve an excuse");
    }

    [Fact]
    public async Task FixedDeferralAction_UsesCustomExcuse()
    {
        // arrange
        var customExcuse = "Waiting for bureaucratic approval.";
        var action = new FixedDeferralAction(TimeSpan.FromMinutes(15), customExcuse);
        var task = new ProcrastinationTask();
        var context = new RuleEvaluationContext(task);

        // act
        var result = await action.ExecuteAsync(context, CancellationToken.None);

        // assert
        result.Excuse.Should().Be(customExcuse, "custom excuse should be used");
    }

    [Fact]
    public async Task ExponentialRegretAction_AppliesRegretFactor()
    {
        // arrange
        var action = new ExponentialRegretAction(TimeSpan.FromMinutes(10), regretMultiplier: 2.0);
        var task = new ProcrastinationTask();
        var context = new RuleEvaluationContext(task)
        {
            RegretFactor = 1.0
        };

        // act
        var result = await action.ExecuteAsync(context, CancellationToken.None);

        // assert
        result.DeferralDuration.Should().Be(TimeSpan.FromMinutes(10), "initial deferral with regret factor 1.0");
        result.RegretMultiplier.Should().Be(2.0, "regret grows exponentially");
    }

    [Fact]
    public async Task ExponentialRegretAction_AccumulatesRegret()
    {
        // arrange
        var action = new ExponentialRegretAction(TimeSpan.FromMinutes(10), regretMultiplier: 1.5);
        var task = new ProcrastinationTask();
        var context = new RuleEvaluationContext(task)
        {
            RegretFactor = 2.0 // previous regret accumulated
        };

        // act
        var result = await action.ExecuteAsync(context, CancellationToken.None);

        // assert - 10 minutes * regret factor of 2.0 = 20 minutes effective deferral
        result.DeferralDuration.Should().Be(TimeSpan.FromMinutes(20), "regret amplifies deferral duration");
    }

    [Fact]
    public async Task RandomDeferralAction_ReturnsWithinRange()
    {
        // arrange
        var action = new RandomDeferralAction(TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(30));
        var task = new ProcrastinationTask();
        var randomProvider = Substitute.For<IRandomProvider>();
        randomProvider.GetDouble().Returns(0.5);
        var context = new RuleEvaluationContext(task, randomProvider: randomProvider);

        // act
        var result = await action.ExecuteAsync(context, CancellationToken.None);

        // assert - with 0.5 random factor: 5 + (30-5)*0.5 = 5 + 12.5 = 17.5 minutes
        result.DeferralDuration.Should().Be(TimeSpan.FromMinutes(17.5), "random factor at 0.5 gives midpoint of range");
    }

    [Fact]
    public async Task BlockTaskAction_SetsBlockingFlag()
    {
        // arrange
        var blockReason = "Pending approval from the Committee of Committees.";
        var action = new BlockTaskAction(blockReason);
        var task = new ProcrastinationTask();
        var context = new RuleEvaluationContext(task);

        // act
        var result = await action.ExecuteAsync(context, CancellationToken.None);

        // assert
        result.ShouldBlock.Should().BeTrue("task should be blocked");
        result.BlockingReason.Should().Be(blockReason, "blocking reason should match");
    }

    [Fact]
    public async Task PanicScalingDeferralAction_ScalesWithDeadlineProximity()
    {
        // arrange
        var action = new PanicScalingDeferralAction(TimeSpan.FromMinutes(10));
        var now = new DateTimeOffset(2025, 1, 1, 12, 0, 0, TimeSpan.Zero);
        var deadline = now.AddHours(6); // 6 hours away

        var timeProvider = Substitute.For<ProcrastiN8.LazyTasks.ITimeProvider>();
        timeProvider.GetUtcNow().Returns(now);

        var task = new ProcrastinationTask { Deadline = deadline };
        var context = new RuleEvaluationContext(task, timeProvider);

        // act
        var result = await action.ExecuteAsync(context, CancellationToken.None);

        // assert - panic factor should be 24/6 = 4.0, so 10 * 4 = 40 minutes
        result.DeferralDuration.Should().Be(TimeSpan.FromMinutes(40), "panic scales inversely with deadline proximity");
    }

    [Fact]
    public async Task ExcuseOnlyAction_GeneratesExcuseWithoutDeferral()
    {
        // arrange
        var excuse = "Mercury is in retrograde.";
        var action = new ExcuseOnlyAction(excuse);
        var task = new ProcrastinationTask();
        var context = new RuleEvaluationContext(task);

        // act
        var result = await action.ExecuteAsync(context, CancellationToken.None);

        // assert
        result.Excuse.Should().Be(excuse, "excuse should be generated");
        result.DeferralDuration.Should().Be(TimeSpan.Zero, "no deferral, just excuses");
    }

    [Fact]
    public async Task ExcuseOnlyAction_GeneratesContextualExcuse()
    {
        // arrange
        var action = new ExcuseOnlyAction(ctx => $"Task '{ctx.Task.Name}' is not ready yet.");
        var task = new ProcrastinationTask { Name = "Important Report" };
        var context = new RuleEvaluationContext(task);

        // act
        var result = await action.ExecuteAsync(context, CancellationToken.None);

        // assert
        result.Excuse.Should().Contain("Important Report", "excuse should reference task name");
    }

    [Fact]
    public void Action_Describe_ProvidesHumanReadableDescription()
    {
        // arrange
        var action = new FixedDeferralAction(TimeSpan.FromMinutes(30));

        // act
        var description = action.Describe();

        // assert
        description.Should().Contain("30", "description should mention the duration");
    }
}
