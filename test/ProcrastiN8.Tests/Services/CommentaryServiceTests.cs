using ProcrastiN8.Services;

namespace ProcrastiN8.Tests.Services;

public class CommentaryServiceTests
{
    [Fact]
    public void LogRandomRemark_DoesNotThrow()
    {
        // Arrange
        var service = new CommentaryService();
        var logger = Substitute.For<IProcrastiLogger>();

        // Act
        var ex = Record.Exception(() => service.LogRandomRemark(logger));

        // Assert
        ex.Should().BeNull();
    }
}