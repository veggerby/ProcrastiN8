using ProcrastiN8.JustBecause;
using ProcrastiN8.LazyTasks;
using ProcrastiN8.RulesEngine;
using ProcrastiN8.RulesEngine.Actions;
using ProcrastiN8.RulesEngine.Conditions;

namespace ProcrastiN8.Tests.RulesEngine;

public class RuleTests
{
    [Fact]
    public async Task ProcrastinationRule_ExecutesActionWhenConditionMatches()
    {
        // arrange
        var rule = new ProcrastinationRule(
            "TEST-001",
            "Test Rule",
            new TagContainsCondition(new[] { "important" }),
            new FixedDeferralAction(TimeSpan.FromMinutes(15)),
            priority: 10
        );

        var task = new ProcrastinationTask
        {
            Tags = new HashSet<string> { "important" }
        };
        var randomProvider = Substitute.For<IRandomProvider>();
        randomProvider.GetDouble().Returns(0.5);
        var context = new RuleEvaluationContext(task, randomProvider: randomProvider);

        // act
        var result = await rule.EvaluateAsync(context, CancellationToken.None);

        // assert
        result.ConditionMatched.Should().BeTrue("task has 'important' tag");
        result.ActionExecuted.Should().BeTrue("action should execute when condition matches");
        result.ActionResult.Should().NotBeNull("action result should be populated");
        result.ActionResult!.DeferralDuration.Should().Be(TimeSpan.FromMinutes(15));
    }

    [Fact]
    public async Task ProcrastinationRule_SkipsActionWhenConditionDoesNotMatch()
    {
        // arrange
        var rule = new ProcrastinationRule(
            "TEST-002",
            "Test Rule",
            new TagContainsCondition(new[] { "critical" }),
            new FixedDeferralAction(TimeSpan.FromMinutes(30)),
            priority: 20
        );

        var task = new ProcrastinationTask
        {
            Tags = new HashSet<string> { "routine" }
        };
        var context = new RuleEvaluationContext(task);

        // act
        var result = await rule.EvaluateAsync(context, CancellationToken.None);

        // assert
        result.ConditionMatched.Should().BeFalse("task does not have 'critical' tag");
        result.ActionExecuted.Should().BeFalse("action should not execute when condition fails");
        result.ActionResult.Should().BeNull("no action result when action not executed");
    }

    [Fact]
    public async Task ProcrastinationRule_UpdatesContextWithActionResults()
    {
        // arrange
        var rule = new ProcrastinationRule(
            "TEST-003",
            "Regret Rule",
            new AlwaysTrueCondition(),
            new ExponentialRegretAction(TimeSpan.FromMinutes(10), regretMultiplier: 1.5)
        );

        var task = new ProcrastinationTask();
        var context = new RuleEvaluationContext(task);
        var initialRegret = context.RegretFactor;

        // act
        await rule.EvaluateAsync(context, CancellationToken.None);

        // assert
        context.AccumulatedDeferral.Should().Be(TimeSpan.FromMinutes(10), "deferral should be accumulated");
        context.RegretFactor.Should().Be(initialRegret * 1.5, "regret factor should be multiplied");
        context.Excuses.Should().NotBeEmpty("excuse should be added to context");
    }

    [Fact]
    public async Task ProcrastinationRule_TracksEvaluationInContext()
    {
        // arrange
        var rule = new ProcrastinationRule(
            "TEST-004",
            "Tracking Rule",
            new AlwaysTrueCondition(),
            new FixedDeferralAction(TimeSpan.FromMinutes(5))
        );

        var task = new ProcrastinationTask();
        var randomProvider = Substitute.For<IRandomProvider>();
        randomProvider.GetDouble().Returns(0.5);
        var context = new RuleEvaluationContext(task, randomProvider: randomProvider);

        // act
        var result = await rule.EvaluateAsync(context, CancellationToken.None);

        // assert
        context.EvaluationTrail.Should().Contain(result, "evaluation result should be added to trail");
    }

    [Fact]
    public async Task ProcrastinationRule_SetsBlockingState()
    {
        // arrange
        var rule = new ProcrastinationRule(
            "TEST-005",
            "Blocking Rule",
            new AlwaysTrueCondition(),
            new BlockTaskAction("Pending eternal review.")
        );

        var task = new ProcrastinationTask();
        var context = new RuleEvaluationContext(task);

        // act
        await rule.EvaluateAsync(context, CancellationToken.None);

        // assert
        context.IsBlocked.Should().BeTrue("task should be blocked");
        context.BlockingReason.Should().Be("Pending eternal review.");
    }

    [Fact]
    public void ProcrastinationRule_HasCorrectProperties()
    {
        // arrange & act
        var rule = new ProcrastinationRule(
            "TEST-006",
            "Property Test Rule",
            new AlwaysTrueCondition(),
            new FixedDeferralAction(TimeSpan.FromMinutes(1)),
            priority: 42
        );

        // assert
        rule.Id.Should().Be("TEST-006");
        rule.Name.Should().Be("Property Test Rule");
        rule.Priority.Should().Be(42);
        rule.Condition.Should().BeOfType<AlwaysTrueCondition>();
        rule.Action.Should().BeOfType<FixedDeferralAction>();
    }

    [Fact]
    public async Task ProcrastinationRule_ProvidesConditionExplanation()
    {
        // arrange
        var rule = new ProcrastinationRule(
            "TEST-007",
            "Explanation Rule",
            new TagContainsCondition(new[] { "urgent" }),
            new FixedDeferralAction(TimeSpan.FromMinutes(5))
        );

        var task = new ProcrastinationTask { Tags = new HashSet<string> { "urgent" } };
        var randomProvider = Substitute.For<IRandomProvider>();
        randomProvider.GetDouble().Returns(0.5);
        var context = new RuleEvaluationContext(task, randomProvider: randomProvider);

        // act
        var result = await rule.EvaluateAsync(context, CancellationToken.None);

        // assert
        result.ConditionExplanation.Should().NotBeNullOrEmpty("explanation should be provided");
        result.ConditionExplanation.Should().Contain("SATISFIED", "condition was satisfied");
    }
}
