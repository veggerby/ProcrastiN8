using ProcrastiN8.Common;

namespace ProcrastiN8.Tests.Common;

public class CommentaryGeneratorTests
{
    [Fact]
    public void GetRandomCommentary_ReturnsNonEmptyString()
    {
        // Arrange
        // (no setup needed)

        // Act
        var result = CommentaryGenerator.GetRandomCommentary();

        // Assert
        result.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public void LogRandomCommentary_DoesNotThrow()
    {
        // Arrange
        // (no setup needed)

        // Act
        var ex = Record.Exception(() => CommentaryGenerator.LogRandomCommentary(null));

        // Assert
        ex.Should().BeNull();
    }
}