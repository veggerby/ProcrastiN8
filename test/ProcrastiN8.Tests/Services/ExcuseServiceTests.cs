using ProcrastiN8.Services;

namespace ProcrastiN8.Tests.Services;

public class ExcuseServiceTests
{
    [Fact]
    public void GenerateExcuse_ReturnsNonEmptyString()
    {
        // arrange
        var service = new ExcuseService();

        // act
        var result = service.GenerateExcuse();

        // assert
        result.Should().NotBeNullOrWhiteSpace();
    }
}