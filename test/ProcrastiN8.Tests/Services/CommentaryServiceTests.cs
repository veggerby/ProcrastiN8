using ProcrastiN8.Services;

namespace ProcrastiN8.Tests.Services;

public class CommentaryServiceTests
{
    [Fact]
    public void LogRandomRemark_DoesNotThrow()
    {
        // arrange
        var service = new CommentaryService();
        var logger = Substitute.For<IProcrastiLogger>();

        // act
        var ex = Record.Exception(() => service.LogRandomRemark(logger));

        // assert
        ex.Should().BeNull();
    }
}