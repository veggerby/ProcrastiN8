using ProcrastiN8.Common;

namespace ProcrastiN8.Tests.Common;

public class CommentaryGeneratorTests
{
    [Fact]
    public void GetRandomCommentary_ReturnsNonEmptyString()
    {
        // arrange
        // (no setup needed)

        // act
        var result = CommentaryGenerator.GetRandomCommentary();

        // assert
        result.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public void LogRandomCommentary_DoesNotThrow()
    {
        // arrange
        // (no setup needed)

        // act
        var ex = Record.Exception(() => CommentaryGenerator.LogRandomCommentary(null));

        // assert
        ex.Should().BeNull();
    }
}