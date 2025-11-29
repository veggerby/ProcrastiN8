using ProcrastiN8.JustBecause;
using ProcrastiN8.LazyTasks;
using ProcrastiN8.RulesEngine;
using ProcrastiN8.RulesEngine.Actions;
using ProcrastiN8.RulesEngine.Conditions;

namespace ProcrastiN8.Tests.RulesEngine;

public class ConditionTests
{
    [Fact]
    public void TagContainsCondition_MatchesWhenTagPresent()
    {
        // arrange
        var condition = new TagContainsCondition(new[] { "important" });
        var task = new ProcrastinationTask { Tags = new HashSet<string> { "important", "urgent" } };
        var context = new RuleEvaluationContext(task);

        // act
        var result = condition.Evaluate(context);

        // assert
        result.Should().BeTrue("the task has the 'important' tag, which matches the condition");
    }

    [Fact]
    public void TagContainsCondition_DoesNotMatchWhenTagMissing()
    {
        // arrange
        var condition = new TagContainsCondition(new[] { "critical" });
        var task = new ProcrastinationTask { Tags = new HashSet<string> { "important", "urgent" } };
        var context = new RuleEvaluationContext(task);

        // act
        var result = condition.Evaluate(context);

        // assert
        result.Should().BeFalse("the task does not have the 'critical' tag");
    }

    [Fact]
    public void TagContainsCondition_MatchAll_RequiresAllTags()
    {
        // arrange
        var condition = new TagContainsCondition(new[] { "important", "urgent" }, matchAll: true);
        var task = new ProcrastinationTask { Tags = new HashSet<string> { "important", "urgent", "critical" } };
        var context = new RuleEvaluationContext(task);

        // act
        var result = condition.Evaluate(context);

        // assert
        result.Should().BeTrue("the task has both 'important' and 'urgent' tags");
    }

    [Fact]
    public void TagContainsCondition_MatchAll_FailsIfOneTagMissing()
    {
        // arrange
        var condition = new TagContainsCondition(new[] { "important", "nonexistent" }, matchAll: true);
        var task = new ProcrastinationTask { Tags = new HashSet<string> { "important" } };
        var context = new RuleEvaluationContext(task);

        // act
        var result = condition.Evaluate(context);

        // assert
        result.Should().BeFalse("the task is missing 'nonexistent' tag");
    }

    [Fact]
    public void PriorityCondition_LessThan_MatchesLowerPriority()
    {
        // arrange
        var condition = new PriorityCondition(50, ComparisonOperator.LessThan);
        var task = new ProcrastinationTask { Priority = 25 };
        var context = new RuleEvaluationContext(task);

        // act
        var result = condition.Evaluate(context);

        // assert
        result.Should().BeTrue("priority 25 is less than threshold 50");
    }

    [Theory]
    [InlineData(ComparisonOperator.LessThan, 25, 50, true)]
    [InlineData(ComparisonOperator.LessThan, 50, 50, false)]
    [InlineData(ComparisonOperator.LessThanOrEqual, 50, 50, true)]
    [InlineData(ComparisonOperator.Equal, 50, 50, true)]
    [InlineData(ComparisonOperator.GreaterThan, 75, 50, true)]
    [InlineData(ComparisonOperator.GreaterThanOrEqual, 50, 50, true)]
    [InlineData(ComparisonOperator.NotEqual, 25, 50, true)]
    public void PriorityCondition_EvaluatesCorrectly(ComparisonOperator op, int taskPriority, int threshold, bool expected)
    {
        // arrange
        var condition = new PriorityCondition(threshold, op);
        var task = new ProcrastinationTask { Priority = taskPriority };
        var context = new RuleEvaluationContext(task);

        // act
        var result = condition.Evaluate(context);

        // assert
        result.Should().Be(expected, $"priority {taskPriority} {op} {threshold} should be {expected}");
    }

    [Fact]
    public void DeadlineProximityCondition_MatchesWhenDeadlineApproaching()
    {
        // arrange
        var condition = new DeadlineProximityCondition(TimeSpan.FromHours(24));
        var timeProvider = Substitute.For<ITimeProvider>();
        timeProvider.GetUtcNow().Returns(new DateTimeOffset(2025, 1, 1, 12, 0, 0, TimeSpan.Zero));

        var task = new ProcrastinationTask
        {
            Deadline = new DateTimeOffset(2025, 1, 1, 20, 0, 0, TimeSpan.Zero) // 8 hours away
        };
        var context = new RuleEvaluationContext(task, timeProvider);

        // act
        var result = condition.Evaluate(context);

        // assert
        result.Should().BeTrue("deadline is within 24 hours");
    }

    [Fact]
    public void DeadlineProximityCondition_DoesNotMatchWhenNoDeadline()
    {
        // arrange
        var condition = new DeadlineProximityCondition(TimeSpan.FromHours(24));
        var task = new ProcrastinationTask { Deadline = null };
        var context = new RuleEvaluationContext(task);

        // act
        var result = condition.Evaluate(context);

        // assert
        result.Should().BeFalse("tasks without deadlines exist in eternal possibility");
    }

    [Fact]
    public void AlwaysTrueCondition_AlwaysReturnsTrue()
    {
        // arrange - the ultimate procrastination condition
        var condition = new AlwaysTrueCondition();
        var task = new ProcrastinationTask();
        var context = new RuleEvaluationContext(task);

        // act
        var result = condition.Evaluate(context);

        // assert
        result.Should().BeTrue("procrastination is inevitable");
    }

    [Fact]
    public void AndCondition_RequiresAllConditionsToMatch()
    {
        // arrange
        var condition = new AndCondition(
            new TagContainsCondition(new[] { "important" }),
            new PriorityCondition(50, ComparisonOperator.LessThan)
        );
        var task = new ProcrastinationTask
        {
            Tags = new HashSet<string> { "important" },
            Priority = 25
        };
        var context = new RuleEvaluationContext(task);

        // act
        var result = condition.Evaluate(context);

        // assert
        result.Should().BeTrue("both conditions are satisfied");
    }

    [Fact]
    public void AndCondition_FailsIfOneConditionFails()
    {
        // arrange
        var condition = new AndCondition(
            new TagContainsCondition(new[] { "important" }),
            new PriorityCondition(50, ComparisonOperator.LessThan)
        );
        var task = new ProcrastinationTask
        {
            Tags = new HashSet<string> { "important" },
            Priority = 75 // fails priority check
        };
        var context = new RuleEvaluationContext(task);

        // act
        var result = condition.Evaluate(context);

        // assert
        result.Should().BeFalse("the priority condition fails");
    }

    [Fact]
    public void OrCondition_MatchesIfAnyConditionMatches()
    {
        // arrange
        var condition = new OrCondition(
            new TagContainsCondition(new[] { "critical" }),
            new TagContainsCondition(new[] { "important" })
        );
        var task = new ProcrastinationTask { Tags = new HashSet<string> { "important" } };
        var context = new RuleEvaluationContext(task);

        // act
        var result = condition.Evaluate(context);

        // assert
        result.Should().BeTrue("at least one condition is satisfied");
    }

    [Fact]
    public void NotCondition_NegatesInnerCondition()
    {
        // arrange
        var condition = new NotCondition(new TagContainsCondition(new[] { "done" }));
        var task = new ProcrastinationTask { Tags = new HashSet<string> { "pending" } };
        var context = new RuleEvaluationContext(task);

        // act
        var result = condition.Evaluate(context);

        // assert
        result.Should().BeTrue("task is NOT done, so negation is true");
    }

    [Fact]
    public void Condition_Explain_ProvidesDetailedExplanation()
    {
        // arrange
        var condition = new TagContainsCondition(new[] { "important" });
        var task = new ProcrastinationTask { Tags = new HashSet<string> { "important" } };
        var context = new RuleEvaluationContext(task);

        // act
        var explanation = condition.Explain(context);

        // assert
        explanation.Should().Contain("SATISFIED", "the condition matched");
        explanation.Should().Contain("important", "the tag should be mentioned");
    }
}
