using ProcrastiN8.JustBecause;
using ProcrastiN8.LazyTasks;

namespace ProcrastiN8.Tests.JustBecause;

/// <summary>
/// Unit tests for <see cref="PointlessChain"/>.
/// </summary>
public class PointlessChainTests
{
    [Fact]
    public async Task ExecuteAsync_Should_LogAndDelay()
    {
        // arrange
        var logger = Substitute.For<IProcrastiLogger>();
        var delayStrategy = Substitute.For<IDelayStrategy>();
        delayStrategy.DelayAsync(Arg.Any<TimeSpan?>(), Arg.Any<TimeSpan?>(), Arg.Any<Func<double, bool>?>(), Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);
        var chain = new PointlessChain(delayStrategy, logger);
        var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(100));
        cts.Token.Register(() => Console.WriteLine("Cancellation token triggered."));

        // act
        var task = chain.StartAsync(cancellationToken: cts.Token);
        await Task.Delay(150); // Allow some time for the chain to run
        cts.Cancel();
        await task; // Ensure the task completes after cancellation

        // assert
        logger.Received().Info(Arg.Is<string>(msg => msg.Contains("PointlessChain step")));
        await delayStrategy.Received().DelayAsync(Arg.Any<TimeSpan?>(), Arg.Any<TimeSpan?>(), Arg.Any<Func<double, bool>?>(), Arg.Any<CancellationToken>());
    }
}