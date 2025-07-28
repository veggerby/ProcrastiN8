using ProcrastiN8.Common;

namespace ProcrastiN8.Tests.Common;

public class ExcuseGeneratorTests
{
    [Fact]
    public void GetRandomExcuse_ReturnsNonEmptyString()
    {
        // arrange
        // (no setup needed)

        // act
        var result = ExcuseGenerator.GetRandomExcuse();

        // assert
        result.Should().NotBeNullOrWhiteSpace();
    }
}