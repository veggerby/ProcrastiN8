using ProcrastiN8.Unproductivity;

namespace ProcrastiN8.Tests.Unproductivity;

public class InfiniteSpinnerTests
{
    [Fact]
    public async Task SpinForeverAsync_LogsAndCancelsGracefully()
    {
        // Arrange
        var loggerSub = Substitute.For<IProcrastiLogger>();
        var cts = new CancellationTokenSource();
        var tickRate = TimeSpan.FromMilliseconds(10);

        // Act
        var spinTask = InfiniteSpinner.SpinForeverAsync(loggerSub, tickRate, cts.Token);
        await Task.Delay(30); // Let it spin a few times
        cts.Cancel();
        await spinTask;

        // Assert
        loggerSub.Received(1).Info(Arg.Is<string>(s => s.Contains("Beginning infinite spin cycle")), Arg.Any<object[]>());
        loggerSub.Received(1).Info(Arg.Is<string>(s => s.Contains("Spin cycle cancelled")), Arg.Any<object[]>());
        loggerSub.Received(1).Info(Arg.Is<string>(s => s.Contains("Total time wasted")), Arg.Any<object[]>());
    }

    [Fact]
    public async Task SpinForeverAsync_HandlesOperationCanceledException()
    {
        // Arrange
        var loggerSub = Substitute.For<IProcrastiLogger>();
        var tickRate = TimeSpan.FromMilliseconds(10);
        var cts = new CancellationTokenSource();

        // Act
        var spinTask = InfiniteSpinner.SpinForeverAsync(loggerSub, tickRate, cts.Token);
        cts.Cancel();
        await spinTask;

        // Assert
        loggerSub.Received().Info(Arg.Is<string>(s => s.Contains("Gracefully stopped spinning.")), Arg.Any<object[]>());
    }
}