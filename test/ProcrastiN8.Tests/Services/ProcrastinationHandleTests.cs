using ProcrastiN8.JustBecause;
using ProcrastiN8.LazyTasks;
using ProcrastiN8.Services;

namespace ProcrastiN8.Tests.Services;

/// <summary>
/// Coverage for <see cref="ProcrastinationHandle"/> status transitions and idempotent control signals.
/// Treats trivial state changes with excessive gravity.
/// </summary>
public class ProcrastinationHandleTests
{
    [Fact]
    public void TriggerNow_First_Call_Sets_Status_And_Returns_True()
    {
        // arrange
        var handle = ProcrastinationScheduler.ScheduleWithHandle(() => Task.CompletedTask, TimeSpan.Zero, ProcrastinationMode.InfiniteEstimation);

        // act
        var first = handle.TryTriggerNow();
        var second = handle.TryTriggerNow();

        // assert
        first.Should().BeTrue("initial trigger should succeed in asserting urgency");
        second.Should().BeFalse("subsequent triggers must acknowledge prior decisiveness");
        handle.Status.Should().Be(ProcrastinationStatus.Triggered);
    }

    [Fact]
    public async Task Abandon_Before_Completion_Sets_Status_And_Flags_Result()
    {
        // arrange
        var handle = ProcrastinationScheduler.ScheduleWithHandle(() => Task.Delay(10), TimeSpan.Zero, ProcrastinationMode.MovingTarget);

        // act
        handle.Abandon();
        var result = await handle.Completion;

        // assert
    result.Abandoned.Should().BeTrue();
    handle.Status.Should().BeOneOf([ProcrastinationStatus.Abandoned, ProcrastinationStatus.Deferring]);
    }

    [Fact]
    public async Task Trigger_After_Abandon_Does_Not_Reanimate_Workflow()
    {
        // arrange
        var handle = ProcrastinationScheduler.ScheduleWithHandle(() => Task.Delay(5), TimeSpan.Zero, ProcrastinationMode.MovingTarget);
        handle.Abandon();

        // act
        var attempted = handle.TryTriggerNow();
        var result = await handle.Completion;

        // assert
    attempted.Should().BeFalse("abandoned workflows resent attempts at productivity");
    result.Abandoned.Should().BeTrue();
    result.Triggered.Should().BeFalse();
    }

    [Fact]
    public async Task TriggerNow_Executes_Async_Task_Without_Blocking_Path()
    {
        // arrange
        var executed = false;
        var handle = ProcrastinationScheduler.ScheduleWithHandle(async () =>
        {
            await Task.Delay(5);
            executed = true;
        }, TimeSpan.Zero, ProcrastinationMode.InfiniteEstimation);

        // act
        handle.TriggerNow();
        var result = await handle.Completion;

        // assert
        executed.Should().BeTrue();
        result.Executed.Should().BeTrue();
        result.Triggered.Should().BeTrue();
    }

    [Fact]
    public async Task ScheduleWithHandle_TaskThrows_FaultsCompletion_WithThatException()
    {
        // arrange — a task so catastrophically unproductive it throws immediately
        var expectedException = new InvalidOperationException("The task has achieved peak procrastination by refusing to run at all.");
        var handle = ProcrastinationScheduler.ScheduleWithHandle(
            () => throw expectedException,
            TimeSpan.Zero,
            ProcrastinationMode.MovingTarget,
            delayStrategy: Substitute.For<IDelayStrategy>(),
            randomProvider: Substitute.For<IRandomProvider>());

        // act — observe the wreckage
        var completionTask = handle.Completion;
        var act = async () => await completionTask;

        // assert — the exception surfaces rather than being silently swallowed
        await act.Should().ThrowAsync<InvalidOperationException>(
            "unexpected exceptions from scheduled tasks must be propagated, not buried");
        completionTask.IsFaulted.Should().BeTrue("the handle's Completion task must be in a faulted state");
        completionTask.Exception!.InnerExceptions.Should().ContainSingle(
            ex => ex == expectedException,
            "the exact exception thrown by the task must be visible to awaiting callers");
    }
}
