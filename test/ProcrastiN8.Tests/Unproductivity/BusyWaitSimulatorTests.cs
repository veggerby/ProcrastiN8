using System.Threading;
using System.Threading.Tasks;
using ProcrastiN8.Unproductivity;
using ProcrastiN8.Services;
using Xunit;

namespace ProcrastiN8.Tests.Unproductivity;

public class BusyWaitSimulatorTests
{
    [Fact]
    public async Task SimulateBusyWaitAsync_LogsAndCancelsGracefully()
    {
        // arrange
        var logger = Substitute.For<IProcrastiLogger>();
        var cts = new CancellationTokenSource();
        var commentary = new CommentaryService();
        BusyWaitSimulator.SetCommentaryService(commentary);

        // act
        var busyTask = BusyWaitSimulator.SimulateBusyWaitAsync(TimeSpan.FromSeconds(2), logger, cts.Token);
        await Task.Delay(50); // Let it run a few cycles
        cts.Cancel();
        await busyTask;

        // assert
        logger.Received().Info(Arg.Any<string>(), Arg.Any<object[]>());
    }
}
