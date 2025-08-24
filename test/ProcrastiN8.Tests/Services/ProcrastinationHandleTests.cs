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
}
