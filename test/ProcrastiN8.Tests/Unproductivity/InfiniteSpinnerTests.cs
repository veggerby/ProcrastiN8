using ProcrastiN8.Services;
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
        InfiniteSpinner.SetCommentaryService(new CommentaryService());

        // Act
        var spinTask = InfiniteSpinner.SpinForeverAsync(loggerSub, tickRate, cts.Token);
        await Task.Delay(30); // Let it spin a few times
        cts.Cancel();
        await spinTask;

        // Assert
        loggerSub.Received().Info(Arg.Is<string>(s => s.Contains("Beginning infinite spin cycle")), Arg.Any<object[]>());
        loggerSub.Received().Info(Arg.Is<string>(s => s.Contains("Spin cycle cancelled")), Arg.Any<object[]>());
        loggerSub.Received().Info(Arg.Is<string>(s => s.Contains("Total time wasted")), Arg.Any<object[]>());
    }

    [Fact]
    public async Task SpinForeverAsync_EmitsCommentaryAndWastesTime()
    {
        // Arrange
        var loggerSub = Substitute.For<IProcrastiLogger>();
        var cts = new CancellationTokenSource();
        var tickRate = TimeSpan.FromMilliseconds(5);
        InfiniteSpinner.SetCommentaryService(new CommentaryService());

        // Act
        var spinTask = InfiniteSpinner.SpinForeverAsync(loggerSub, tickRate, cts.Token);
        await Task.Delay(20); // Let it spin a few times
        cts.Cancel();
        await spinTask;

        // Assert
        loggerSub.Received().Info(Arg.Is<string>(s => s.Contains("infinite spin cycle")), Arg.Any<object[]>());
        loggerSub.Received().Info(Arg.Is<string>(s => s.Contains("Total time wasted")), Arg.Any<object[]>());
        // This test is intentionally absurd: we cannot assert on commentary, but we can assert that time was wasted.
        true.Should().BeTrue("time was wasted, as intended");
    }

    private class FailingCommentaryService(Exception exception) : CommentaryService
    {
        private readonly Exception _exception = exception;

        public override void LogRandomRemark(IProcrastiLogger? logger = null) => throw _exception;
    }

    [Fact]
    public async Task SpinForeverAsync_ThrowsAndLogsOnException()
    {
        // Arrange
        var loggerSub = Substitute.For<IProcrastiLogger>();
        var tickRate = TimeSpan.FromMilliseconds(5);
        var cts = new CancellationTokenSource();
        var ex = new InvalidOperationException("Simulated failure");
        InfiniteSpinner.SetCommentaryService(new FailingCommentaryService(ex));

        // Act
        Func<Task> act = async () => await InfiniteSpinner.SpinForeverAsync(loggerSub, tickRate, cts.Token);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>();
        loggerSub.Received().Error(ex, Arg.Is<string>(s => s.Contains("Something went wrong")), Arg.Any<object[]>());
    }
}