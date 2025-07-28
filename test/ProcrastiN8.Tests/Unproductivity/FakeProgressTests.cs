using ProcrastiN8.Services;
using ProcrastiN8.Unproductivity;

namespace ProcrastiN8.Tests.Unproductivity;

public class FakeProgressTests
{
    [Fact]
    public async Task ShowProgressAsync_LogsStagesAndCancelsGracefully()
    {
        // arrange
        var logger = Substitute.For<IProcrastiLogger>();
        var cts = new CancellationTokenSource();
        var commentary = new CommentaryService();
        FakeProgress.SetCommentaryService(commentary);

        // act
        var progressTask = FakeProgress.ShowFakeProgressAsync(logger: logger, cancellationToken: cts.Token);
        await Task.Delay(100); // Let it run a few stages
        cts.Cancel();
        await progressTask;

        // assert
        logger.Received().Info(Arg.Any<string>(), Arg.Any<object[]>());
    }
}