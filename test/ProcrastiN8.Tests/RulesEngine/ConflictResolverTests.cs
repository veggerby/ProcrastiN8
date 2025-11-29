using ProcrastiN8.JustBecause;
using ProcrastiN8.RulesEngine;
using ProcrastiN8.RulesEngine.Actions;
using ProcrastiN8.RulesEngine.Conditions;

namespace ProcrastiN8.Tests.RulesEngine;

public class ConflictResolverTests
{
    [Fact]
    public void CircularConflictResolver_HandlesEmptyResults()
    {
        // arrange
        var resolver = new CircularConflictResolver();
        var results = new List<RuleEvaluationResult>();
        var task = new ProcrastinationTask();
        var context = new RuleEvaluationContext(task);

        // act
        var (resolvedResult, cycles) = resolver.ResolveConflicts(results, context);

        // assert
        cycles.Should().Be(0, "no negotiation needed for empty results");
        resolvedResult.DeferralDuration.Should().Be(TimeSpan.Zero);
    }

    [Fact]
    public void CircularConflictResolver_HandlesSingleResult()
    {
        // arrange
        var resolver = new CircularConflictResolver();
        var rule = new ProcrastinationRule("R1", "Rule 1", new AlwaysTrueCondition(), new FixedDeferralAction(TimeSpan.FromMinutes(10)));
        var results = new List<RuleEvaluationResult>
        {
            new RuleEvaluationResult(rule)
            {
                ConditionMatched = true,
                ActionExecuted = true,
                ActionResult = new RuleActionResult { DeferralDuration = TimeSpan.FromMinutes(10), Excuse = "Single excuse" }
            }
        };
        var task = new ProcrastinationTask();
        var context = new RuleEvaluationContext(task);

        // act
        var (resolvedResult, cycles) = resolver.ResolveConflicts(results, context);

        // assert
        cycles.Should().Be(0, "no negotiation needed for single result");
        resolvedResult.DeferralDuration.Should().Be(TimeSpan.FromMinutes(10), "single result should pass through");
    }

    [Fact]
    public void CircularConflictResolver_NegotiatesBetweenMultipleResults()
    {
        // arrange
        var resolver = new CircularConflictResolver();
        var rule1 = new ProcrastinationRule("R1", "Rule 1", new AlwaysTrueCondition(), new FixedDeferralAction(TimeSpan.FromMinutes(10)));
        var rule2 = new ProcrastinationRule("R2", "Rule 2", new AlwaysTrueCondition(), new FixedDeferralAction(TimeSpan.FromMinutes(20)));

        var results = new List<RuleEvaluationResult>
        {
            new RuleEvaluationResult(rule1)
            {
                ConditionMatched = true,
                ActionExecuted = true,
                ActionResult = new RuleActionResult { DeferralDuration = TimeSpan.FromMinutes(10), Excuse = "Excuse 1" }
            },
            new RuleEvaluationResult(rule2)
            {
                ConditionMatched = true,
                ActionExecuted = true,
                ActionResult = new RuleActionResult { DeferralDuration = TimeSpan.FromMinutes(20), Excuse = "Excuse 2" }
            }
        };
        var task = new ProcrastinationTask();
        var randomProvider = Substitute.For<IRandomProvider>();
        randomProvider.GetDouble().Returns(0.1); // Small random for consistent test results
        var context = new RuleEvaluationContext(task, randomProvider: randomProvider);

        // act
        var (resolvedResult, cycles) = resolver.ResolveConflicts(results, context);

        // assert
        cycles.Should().BeGreaterThan(0, "circular negotiation should occur");
        resolvedResult.DeferralDuration.Should().BeGreaterThan(TimeSpan.Zero, "resolved deferral should be positive");
        resolvedResult.Excuse.Should().Contain("negotiation", "excuse should mention negotiation");
    }

    [Fact]
    public void CircularConflictResolver_AccumulatesRegretMultipliers()
    {
        // arrange
        var resolver = new CircularConflictResolver();
        var rule1 = new ProcrastinationRule("R1", "Rule 1", new AlwaysTrueCondition(), new ExponentialRegretAction(TimeSpan.FromMinutes(5), 1.5));
        var rule2 = new ProcrastinationRule("R2", "Rule 2", new AlwaysTrueCondition(), new ExponentialRegretAction(TimeSpan.FromMinutes(5), 2.0));

        var results = new List<RuleEvaluationResult>
        {
            new RuleEvaluationResult(rule1)
            {
                ConditionMatched = true,
                ActionExecuted = true,
                ActionResult = new RuleActionResult { DeferralDuration = TimeSpan.FromMinutes(5), RegretMultiplier = 1.5 }
            },
            new RuleEvaluationResult(rule2)
            {
                ConditionMatched = true,
                ActionExecuted = true,
                ActionResult = new RuleActionResult { DeferralDuration = TimeSpan.FromMinutes(5), RegretMultiplier = 2.0 }
            }
        };
        var task = new ProcrastinationTask();
        var randomProvider = Substitute.For<IRandomProvider>();
        randomProvider.GetDouble().Returns(0.1);
        var context = new RuleEvaluationContext(task, randomProvider: randomProvider);

        // act
        var (resolvedResult, cycles) = resolver.ResolveConflicts(results, context);

        // assert - regret multipliers compound: 1.5 * 2.0 = 3.0 (applied per cycle)
        resolvedResult.RegretMultiplier.Should().BeGreaterThan(1.0, "regret should accumulate");
    }

    [Fact]
    public void CircularConflictResolver_PropagatesBlockingState()
    {
        // arrange
        var resolver = new CircularConflictResolver();
        var rule1 = new ProcrastinationRule("R1", "Rule 1", new AlwaysTrueCondition(), new FixedDeferralAction(TimeSpan.FromMinutes(10)));
        var rule2 = new ProcrastinationRule("R2", "Blocking Rule", new AlwaysTrueCondition(), new BlockTaskAction("Critical block"));

        var results = new List<RuleEvaluationResult>
        {
            new RuleEvaluationResult(rule1)
            {
                ConditionMatched = true,
                ActionExecuted = true,
                ActionResult = new RuleActionResult { DeferralDuration = TimeSpan.FromMinutes(10) }
            },
            new RuleEvaluationResult(rule2)
            {
                ConditionMatched = true,
                ActionExecuted = true,
                ActionResult = new RuleActionResult { ShouldBlock = true, BlockingReason = "Critical block" }
            }
        };
        var task = new ProcrastinationTask();
        var randomProvider = Substitute.For<IRandomProvider>();
        randomProvider.GetDouble().Returns(0.1);
        var context = new RuleEvaluationContext(task, randomProvider: randomProvider);

        // act
        var (resolvedResult, _) = resolver.ResolveConflicts(results, context);

        // assert
        resolvedResult.ShouldBlock.Should().BeTrue("blocking state should propagate");
        resolvedResult.BlockingReason.Should().Be("Critical block");
    }

    [Fact]
    public void CircularConflictResolver_RespectsMaxCycles()
    {
        // arrange
        var resolver = new CircularConflictResolver();
        var rule1 = new ProcrastinationRule("R1", "Rule 1", new AlwaysTrueCondition(), new FixedDeferralAction(TimeSpan.FromHours(1)));
        var rule2 = new ProcrastinationRule("R2", "Rule 2", new AlwaysTrueCondition(), new FixedDeferralAction(TimeSpan.FromHours(2)));

        var results = new List<RuleEvaluationResult>
        {
            new RuleEvaluationResult(rule1)
            {
                ConditionMatched = true,
                ActionExecuted = true,
                ActionResult = new RuleActionResult { DeferralDuration = TimeSpan.FromHours(1) }
            },
            new RuleEvaluationResult(rule2)
            {
                ConditionMatched = true,
                ActionExecuted = true,
                ActionResult = new RuleActionResult { DeferralDuration = TimeSpan.FromHours(2) }
            }
        };
        var task = new ProcrastinationTask();
        var randomProvider = Substitute.For<IRandomProvider>();
        randomProvider.GetDouble().Returns(0.99); // Large random to prevent convergence
        var context = new RuleEvaluationContext(task, randomProvider: randomProvider);

        // act
        var (_, cycles) = resolver.ResolveConflicts(results, context, maxCycles: 5);

        // assert
        cycles.Should().BeLessThanOrEqualTo(5, "negotiation should stop at max cycles");
    }

    [Fact]
    public void CircularConflictResolver_SkipsResultsWithoutActionResults()
    {
        // arrange
        var resolver = new CircularConflictResolver();
        var rule1 = new ProcrastinationRule("R1", "Rule 1", new AlwaysTrueCondition(), new FixedDeferralAction(TimeSpan.FromMinutes(10)));
        var rule2 = new ProcrastinationRule("R2", "Rule 2", new TagContainsCondition(new[] { "never" }), new FixedDeferralAction(TimeSpan.FromMinutes(20)));

        var results = new List<RuleEvaluationResult>
        {
            new RuleEvaluationResult(rule1)
            {
                ConditionMatched = true,
                ActionExecuted = true,
                ActionResult = new RuleActionResult { DeferralDuration = TimeSpan.FromMinutes(10) }
            },
            new RuleEvaluationResult(rule2)
            {
                ConditionMatched = false,
                ActionExecuted = false,
                ActionResult = null // No action result because condition didn't match
            }
        };
        var task = new ProcrastinationTask();
        var randomProvider = Substitute.For<IRandomProvider>();
        randomProvider.GetDouble().Returns(0.1);
        var context = new RuleEvaluationContext(task, randomProvider: randomProvider);

        // act - should not throw, should handle gracefully
        var (resolvedResult, cycles) = resolver.ResolveConflicts(results, context);

        // assert - only the first result should contribute
        cycles.Should().Be(0, "only one valid result means no conflict to resolve");
        resolvedResult.DeferralDuration.Should().Be(TimeSpan.FromMinutes(10));
    }
}
