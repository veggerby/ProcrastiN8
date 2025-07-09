using ProcrastiN8.Services;

namespace ProcrastiN8.Tests.Services;

public class DelayServiceTests
{
    [Fact]
    public async Task DelayWithProcrastinationAsync_DoesNotThrow()
    {
        // Arrange
        var service = new DelayService();

        // Act
        var ex = await Record.ExceptionAsync(() => service.DelayWithProcrastinationAsync("test", System.TimeSpan.FromMilliseconds(10), new System.Threading.CancellationToken()));

        // Assert
        ex.Should().BeNull();
    }
}