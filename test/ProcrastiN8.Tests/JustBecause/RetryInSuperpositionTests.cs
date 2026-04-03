using ProcrastiN8.JustBecause;

namespace ProcrastiN8.Tests.JustBecause;

public class RetryInSuperpositionTests
{
    [Fact]
    public async Task ExecuteAsync_WithSuccessfulOperation_ReturnsResult()
    {
        // arrange — all timelines succeed; the first collapse wins
        var callCount = 0;

        // act
        var result = await RetryInSuperposition.ExecuteAsync(
            () => { callCount++; return Task.FromResult(42); },
            maxAttempts: 3);

        // assert — a value emerges from the superposition
        result.Should().Be(42, "the first successful timeline collapses reality to the correct value");
        callCount.Should().BeGreaterThanOrEqualTo(1, "at least one timeline must have executed");
    }

    [Fact]
    public async Task ExecuteAsync_AllFail_ThrowsAggregateException()
    {
        // arrange — all timelines explode simultaneously
        Func<Task<int>> alwaysFails = () => throw new InvalidOperationException("This timeline is not viable.");

        // act
        Func<Task> act = () => RetryInSuperposition.ExecuteAsync(alwaysFails, maxAttempts: 3);

        // assert — the universe admits defeat
        await act.Should().ThrowAsync<AggregateException>("all timelines collapsing simultaneously is the expected failure mode");
    }

    [Fact]
    public async Task ExecuteAsync_Action_Succeeds_WhenOperationSucceeds()
    {
        // arrange
        var executed = false;

        // act
        await RetryInSuperposition.ExecuteAsync(
            () => { executed = true; return Task.CompletedTask; },
            maxAttempts: 2);

        // assert
        executed.Should().BeTrue("the action executes across at least one timeline");
    }

    [Fact]
    public async Task ExecuteAsync_ZeroAttempts_ThrowsArgumentOutOfRange()
    {
        // arrange + act
        Func<Task> act = () => RetryInSuperposition.ExecuteAsync(
            () => Task.FromResult(1),
            maxAttempts: 0);

        // assert
        await act.Should().ThrowAsync<ArgumentOutOfRangeException>("zero timelines means no attempt was ever made — even for us, that's too little");
    }

    [Fact]
    public async Task ExecuteAsync_RespectsCancellation()
    {
        // arrange
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        Func<Task<int>> neverCompletes = async () =>
        {
            await Task.Delay(Timeout.Infinite, cts.Token);
            return 0;
        };

        // act
        Func<Task> act = () => RetryInSuperposition.ExecuteAsync(neverCompletes, maxAttempts: 2, cancellationToken: cts.Token);

        // assert — cancellation collapses all timelines simultaneously
        await act.Should().ThrowAsync<OperationCanceledException>("cancellation is the one thing that transcends superposition");
    }
}
