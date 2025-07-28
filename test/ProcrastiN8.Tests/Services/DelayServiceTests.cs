using ProcrastiN8.Services;

namespace ProcrastiN8.Tests.Services;

public class DelayServiceTests
{
    [Fact]
    public async Task DelayWithProcrastinationAsync_DoesNotThrow()
    {
        // arrange
        var service = new DelayService();

        // act
        var ex = await Record.ExceptionAsync(() => service.DelayWithProcrastinationAsync("test", System.TimeSpan.FromMilliseconds(10), new System.Threading.CancellationToken()));

        // assert
        ex.Should().BeNull();
    }
}