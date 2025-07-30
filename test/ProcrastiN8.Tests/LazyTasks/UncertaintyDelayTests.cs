using ProcrastiN8.Common;
using ProcrastiN8.JustBecause;
using ProcrastiN8.LazyTasks;

namespace ProcrastiN8.Tests.LazyTasks;

public class UncertaintyDelayTests
{
    [Fact]
    public async Task WaitAsync_Should_Delay_For_Random_Duration()
    {
        // arrange
        var randomProvider = Substitute.For<IRandomProvider>();
        randomProvider.GetDouble().Returns(0.5); // Always return 50% of max delay

        var excuseProvider = Substitute.For<IExcuseProvider>();
        excuseProvider.GetExcuseAsync().Returns(Task.FromResult("Testing delay"));

        var maxDelay = TimeSpan.FromMilliseconds(1000);

        // act
        await UncertaintyDelay.WaitAsync(maxDelay, randomProvider: randomProvider, excuseProvider: excuseProvider, cancellationToken: CancellationToken.None);

        // assert
        await excuseProvider.Received().GetExcuseAsync();
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
}