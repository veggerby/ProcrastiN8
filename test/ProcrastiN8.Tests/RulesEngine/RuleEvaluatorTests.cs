using ProcrastiN8.JustBecause;
using ProcrastiN8.RulesEngine;
using ProcrastiN8.RulesEngine.Actions;
using ProcrastiN8.RulesEngine.Conditions;
using ProcrastiN8.RulesEngine.Policies;

namespace ProcrastiN8.Tests.RulesEngine;

public class RuleEvaluatorTests
{
    [Fact]
    public async Task ProcrastinationRuleEvaluator_EvaluatesAllRules()
    {
        // arrange
        var host = new PolicyHost();
        var policy = new TestPolicyPack();
        policy.AddRule(new ProcrastinationRule("R1", "Rule 1", new AlwaysTrueCondition(), new FixedDeferralAction(TimeSpan.FromMinutes(5)), 10));
        policy.AddRule(new ProcrastinationRule("R2", "Rule 2", new AlwaysTrueCondition(), new FixedDeferralAction(TimeSpan.FromMinutes(10)), 20));
        host.LoadPolicy(policy);

        var evaluator = new ProcrastinationRuleEvaluator(host);
        var task = new ProcrastinationTask();
        var randomProvider = Substitute.For<IRandomProvider>();
        randomProvider.GetDouble().Returns(0.5);
        var context = new RuleEvaluationContext(task, randomProvider: randomProvider);

        // act
        var result = await evaluator.EvaluateAsync(context, CancellationToken.None);

        // assert
        result.RulesEvaluated.Should().Be(2, "both rules should be evaluated");
        result.RulesMatched.Should().Be(2, "both rules have AlwaysTrue conditions");
    }

    [Fact]
    public async Task ProcrastinationRuleEvaluator_AccumulatesDeferral()
    {
        // arrange
        var host = new PolicyHost();
        var policy = new TestPolicyPack();
        policy.AddRule(new ProcrastinationRule("R1", "Rule 1", new AlwaysTrueCondition(), new FixedDeferralAction(TimeSpan.FromMinutes(5)), 10));
        host.LoadPolicy(policy);

        var evaluator = new ProcrastinationRuleEvaluator(host);
        var task = new ProcrastinationTask();
        var randomProvider = Substitute.For<IRandomProvider>();
        randomProvider.GetDouble().Returns(0.5);
        var context = new RuleEvaluationContext(task, randomProvider: randomProvider);

        // act
        var result = await evaluator.EvaluateAsync(context, CancellationToken.None);

        // assert
        result.TotalDeferral.Should().BeGreaterThan(TimeSpan.Zero, "deferral should be accumulated");
    }

    [Fact]
    public async Task ProcrastinationRuleEvaluator_ResolvesConflictsWithMultipleMatchingRules()
    {
        // arrange
        var host = new PolicyHost();
        var policy = new TestPolicyPack();
        policy.AddRule(new ProcrastinationRule("R1", "Rule 1", new AlwaysTrueCondition(), new FixedDeferralAction(TimeSpan.FromMinutes(10)), 10));
        policy.AddRule(new ProcrastinationRule("R2", "Rule 2", new AlwaysTrueCondition(), new FixedDeferralAction(TimeSpan.FromMinutes(20)), 20));
        host.LoadPolicy(policy);

        var evaluator = new ProcrastinationRuleEvaluator(host);
        var task = new ProcrastinationTask();
        var randomProvider = Substitute.For<IRandomProvider>();
        randomProvider.GetDouble().Returns(0.1);
        var context = new RuleEvaluationContext(task, randomProvider: randomProvider);

        // act
        var result = await evaluator.EvaluateAsync(context, CancellationToken.None);

        // assert
        result.ConflictResolutionCycles.Should().BeGreaterThan(0, "conflict resolution should occur with multiple matching rules");
        result.TotalDeferral.Should().BeGreaterThan(TimeSpan.Zero, "resolved deferral should be calculated");
    }

    [Fact]
    public async Task ProcrastinationRuleEvaluator_CollectsExcuses()
    {
        // arrange
        var host = new PolicyHost();
        var policy = new TestPolicyPack();
        policy.AddRule(new ProcrastinationRule("R1", "Rule 1", new AlwaysTrueCondition(), new ExcuseOnlyAction("Excuse 1"), 10));
        policy.AddRule(new ProcrastinationRule("R2", "Rule 2", new AlwaysTrueCondition(), new ExcuseOnlyAction("Excuse 2"), 20));
        host.LoadPolicy(policy);

        var evaluator = new ProcrastinationRuleEvaluator(host);
        var task = new ProcrastinationTask();
        var context = new RuleEvaluationContext(task);

        // act
        var result = await evaluator.EvaluateAsync(context, CancellationToken.None);

        // assert
        result.Excuses.Should().NotBeEmpty("excuses should be collected");
    }

    [Fact]
    public async Task ProcrastinationRuleEvaluator_SetsBlockingState()
    {
        // arrange
        var host = new PolicyHost();
        var policy = new TestPolicyPack();
        policy.AddRule(new ProcrastinationRule("R1", "Blocking Rule", new AlwaysTrueCondition(), new BlockTaskAction("Blocked!"), 10));
        host.LoadPolicy(policy);

        var evaluator = new ProcrastinationRuleEvaluator(host);
        var task = new ProcrastinationTask();
        var context = new RuleEvaluationContext(task);

        // act
        var result = await evaluator.EvaluateAsync(context, CancellationToken.None);

        // assert
        result.IsBlocked.Should().BeTrue("task should be blocked");
        result.BlockingReason.Should().Be("Blocked!");
    }

    [Fact]
    public async Task ProcrastinationRuleEvaluator_TracksRegretFactor()
    {
        // arrange
        var host = new PolicyHost();
        var policy = new TestPolicyPack();
        policy.AddRule(new ProcrastinationRule("R1", "Regret Rule", new AlwaysTrueCondition(), new ExponentialRegretAction(TimeSpan.FromMinutes(5), 1.5), 10));
        host.LoadPolicy(policy);

        var evaluator = new ProcrastinationRuleEvaluator(host);
        var task = new ProcrastinationTask();
        var context = new RuleEvaluationContext(task);

        // act
        var result = await evaluator.EvaluateAsync(context, CancellationToken.None);

        // assert
        result.FinalRegretFactor.Should().BeGreaterThan(1.0, "regret factor should accumulate");
    }

    [Fact]
    public async Task ProcrastinationRuleEvaluator_HandlesCancellation()
    {
        // arrange
        var host = new PolicyHost();
        var policy = new TestPolicyPack();
        policy.AddRule(new ProcrastinationRule("R1", "Rule 1", new AlwaysTrueCondition(), new FixedDeferralAction(TimeSpan.FromMinutes(5)), 10));
        host.LoadPolicy(policy);

        var evaluator = new ProcrastinationRuleEvaluator(host);
        var task = new ProcrastinationTask();
        var randomProvider = Substitute.For<IRandomProvider>();
        randomProvider.GetDouble().Returns(0.5);
        var context = new RuleEvaluationContext(task, randomProvider: randomProvider);
        var cts = new CancellationTokenSource();
        cts.Cancel();

        // act & assert
        await FluentActions
            .Awaiting(async () => await evaluator.EvaluateAsync(context, cts.Token))
            .Should().ThrowAsync<OperationCanceledException>("cancellation should be respected");
    }

    [Fact]
    public void ProcrastinationRuleEvaluator_GeneratesExplainabilityReport()
    {
        // arrange
        var host = new PolicyHost();
        var evaluator = new ProcrastinationRuleEvaluator(host);
        var task = new ProcrastinationTask { Name = "Test Task" };

        var result = new RuleEngineResult(task)
        {
            TotalDeferral = TimeSpan.FromMinutes(30),
            RulesMatched = 3,
            RulesEvaluated = 5,
            ConflictResolutionCycles = 2,
            FinalRegretFactor = 1.5
        };
        result.Excuses.Add("Test excuse 1");
        result.Excuses.Add("Test excuse 2");

        // act
        var report = evaluator.GenerateReport(result);

        // assert
        report.Should().NotBeNull();
        report.Title.Should().Contain("Test Task");
        report.ExecutiveSummary.Should().NotBeNullOrEmpty();
        report.TaskSummary.Should().NotBeNullOrEmpty();
        report.FinalRecommendation.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void ExplainabilityReport_ToFullReport_GeneratesVerboseOutput()
    {
        // arrange
        var report = new ExplainabilityReport
        {
            Title = "Test Report",
            ExecutiveSummary = "This is the executive summary.",
            TaskSummary = "Task details go here.",
            FinalRecommendation = "Do nothing. Immediately.",
            Appendix = "Additional details..."
        };

        report.RuleExplanations.Add(new RuleExplanationSection
        {
            RuleId = "TEST-001",
            RuleName = "Test Rule",
            Priority = 10,
            ConditionMatched = true,
            DetailedExplanation = "The condition matched because reasons.",
            Rationale = "This is entirely justified by bureaucracy."
        });

        report.ExcuseJustifications.Add(new ExcuseJustification
        {
            Excuse = "The dog ate my productivity.",
            SourceRule = "TEST-001",
            PhilosophicalBasis = "According to Nietzsche, productivity is an illusion anyway."
        });

        // act
        var fullReport = report.ToFullReport();

        // assert
        fullReport.Should().NotBeNullOrEmpty();
        fullReport.Should().Contain("Test Report");
        fullReport.Should().Contain("EXECUTIVE SUMMARY");
        fullReport.Should().Contain("RULE-BY-RULE ANALYSIS");
        fullReport.Should().Contain("TEST-001");
        fullReport.Should().Contain("dog ate my productivity");
        fullReport.Should().Contain("FINAL RECOMMENDATION");
    }

    [Fact]
    public void ExplainabilityReport_EstimatedPageCount_AimsFor12Pages()
    {
        // arrange - even minimal reports aim for 12 pages
        var report = new ExplainabilityReport();

        // act & assert
        report.EstimatedPageCount.Should().BeGreaterThanOrEqualTo(12, "compliance reports must be suitably verbose");
    }

    private sealed class TestPolicyPack : PolicyPackBase
    {
        public override string Id => "TEST-POLICY";
        public override string Name => "Test Policy";
        public override string Description => "A test policy pack.";
        public override Version Version => new Version(1, 0, 0);

        /// <summary>
        /// Exposes the protected AddRule method for testing purposes.
        /// The 'new' modifier changes visibility from protected to public.
        /// </summary>
        public new void AddRule(IRule rule)
        {
            base.AddRule(rule);
        }
    }
}
