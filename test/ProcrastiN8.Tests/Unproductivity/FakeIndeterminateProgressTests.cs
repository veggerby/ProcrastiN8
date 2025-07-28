using ProcrastiN8.Services;
using ProcrastiN8.Unproductivity;

namespace ProcrastiN8.Tests.Unproductivity;

public class FakeIndeterminateProgressTests
{
    [Fact]
    public async Task ShowIndeterminateProgressAsync_LogsAndCancelsGracefully()
    {
        // arrange
        var logger = Substitute.For<IProcrastiLogger>();
        var cts = new CancellationTokenSource();
        var commentary = new CommentaryService();
        FakeIndeterminateProgress.SetCommentaryService(commentary);

        // act
        var progressTask = FakeIndeterminateProgress.ShowAsync(logger, cancellationToken: cts.Token);
        await Task.Delay(50); // Let it run a few cycles
        cts.Cancel();
        await progressTask;

        // assert
        logger.Received().Info(Arg.Any<string>(), Arg.Any<object[]>());
    }
}