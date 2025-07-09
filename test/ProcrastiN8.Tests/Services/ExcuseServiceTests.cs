using ProcrastiN8.Services;

namespace ProcrastiN8.Tests.Services;

public class ExcuseServiceTests
{
    [Fact]
    public void GenerateExcuse_ReturnsNonEmptyString()
    {
        // Arrange
        var service = new ExcuseService();

        // Act
        var result = service.GenerateExcuse();

        // Assert
        result.Should().NotBeNullOrWhiteSpace();
    }
}