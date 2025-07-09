using ProcrastiN8.Common;

namespace ProcrastiN8.Tests.Common;

public class ExcuseGeneratorTests
{
    [Fact]
    public void GetRandomExcuse_ReturnsNonEmptyString()
    {
        // Arrange
        // (no setup needed)

        // Act
        var result = ExcuseGenerator.GetRandomExcuse();

        // Assert
        result.Should().NotBeNullOrWhiteSpace();
    }
}