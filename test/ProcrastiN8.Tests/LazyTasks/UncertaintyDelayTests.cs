using ProcrastiN8.Common;
using ProcrastiN8.JustBecause;
using ProcrastiN8.LazyTasks;

namespace ProcrastiN8.Tests.LazyTasks;

public class UncertaintyDelayTests
{
    [Fact]
    public async Task WaitAsync_Should_Delay_For_Random_Duration_And_Use_Excuses()
    {
        // arrange
        var excuseProvider = Substitute.For<IExcuseProvider>();
        excuseProvider.GetExcuseAsync().Returns(Task.FromResult("Testing delay"));

        var capturedDelays = new List<TimeSpan>();
        var delayProvider = new FakeDelayProvider(capturedDelays);

        // random sequence: first value (0.50) chooses rounds (explicitly provided below so ignored for count), then per-round delays (0.40, 0.60, 0.80)
        var randomProvider = new SequenceRandomProvider(0.50, 0.40, 0.60, 0.80);

        var maxDelay = TimeSpan.FromMilliseconds(1000);
        var rounds = 3;

        // act
        await UncertaintyDelay.WaitAsync(maxDelay, rounds: rounds, randomProvider: randomProvider, excuseProvider: excuseProvider, delayProvider: delayProvider, cancellationToken: CancellationToken.None);

        // assert
        capturedDelays.Count.Should().Be(rounds, "each round should invoke the delay provider exactly once");
        await excuseProvider.Received(rounds).GetExcuseAsync();
        capturedDelays.Should().AllSatisfy(d => d.Should().BeLessThanOrEqualTo(maxDelay));
    }

    [Fact]
    public async Task WaitAsync_Should_Fallback_To_Random_Round_Count_When_Rounds_Is_Zero()
    {
        // arrange
        // First double (0.50) -> rounds calculation: Min=2, Max=6 => (0.50 * 4) = 2 -> floor = 2 -> 2 + 2 = 4 rounds
        // Subsequent doubles for each delay computation
        var randomProvider = new SequenceRandomProvider(0.50, 0.10, 0.20, 0.30, 0.40);
        var excuseProvider = Substitute.For<IExcuseProvider>();
        excuseProvider.GetExcuseAsync().Returns(Task.FromResult<string>("Because uncertainty demanded it."));

        var capturedDelays = new List<TimeSpan>();
        var delayProvider = new FakeDelayProvider(capturedDelays);

        // act
        await UncertaintyDelay.WaitAsync(TimeSpan.FromMilliseconds(500), rounds: 0, randomProvider: randomProvider, excuseProvider: excuseProvider, delayProvider: delayProvider);

        // assert
        capturedDelays.Count.Should().Be(4, "rounds=0 should trigger random selection which produced 4 rounds");
        await excuseProvider.Received(4).GetExcuseAsync();
    }

    [Fact]
    public async Task WaitAsync_Should_Throw_When_Canceled()
    {
        // arrange
        var cts = new CancellationTokenSource();
        cts.Cancel();

        // act
        Func<Task> act = async () => await UncertaintyDelay.WaitAsync(TimeSpan.FromMilliseconds(1000), cancellationToken: cts.Token);

        // assert
        await act.Should().ThrowAsync<OperationCanceledException>();
    }

    [Fact]
    public async Task WaitAsync_Should_Throw_If_MaxDelay_Is_Zero_Or_Negative()
    {
        // arrange
        var invalidDelay = TimeSpan.Zero;

        // act
        Func<Task> act = async () => await UncertaintyDelay.WaitAsync(invalidDelay);

        // assert
        await act.Should().ThrowAsync<ArgumentOutOfRangeException>().WithMessage("*Maximum delay must be greater than zero.*");
    }

    private sealed class FakeDelayProvider(List<TimeSpan> captures) : IDelayProvider
    {
        public Task DelayAsync(TimeSpan delay, CancellationToken cancellationToken = default)
        {
            captures.Add(delay);
            return Task.CompletedTask; // Intentionally instantaneous; we procrastinate conceptually only.
        }
    }

    private sealed class SequenceRandomProvider : IRandomProvider
    {
        private readonly Queue<double> _values;

        public SequenceRandomProvider(params double[] values)
        {
            _values = new Queue<double>(values);
        }

        public double GetDouble()
        {
            if (_values.Count == 0)
            {
                // Deterministic fallback to midpoint uncertainty.
                return 0.5;
            }

            return _values.Dequeue();
        }
    }
}