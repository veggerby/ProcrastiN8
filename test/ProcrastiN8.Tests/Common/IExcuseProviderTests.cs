using ProcrastiN8.Common;

namespace ProcrastiN8.Tests.Common;

public class IExcuseProviderTests
{
    [Fact]
    public void GetExcuse_Returns_Custom_Excuse()
    {
        // arrange
        var provider = new ExcuseProvider(() => "The coffee machine is updating.");

        // act
        var excuse = provider.GetExcuse();

        // assert
        excuse.Should().Be("The coffee machine is updating.", "custom excuses are the backbone of plausible deniability");
    }

    [Fact]
    public void GetExcuse_Delegates_To_ExcuseGenerator()
    {
        // arrange
        var provider = new ExcuseProvider();

        // act
        var excuse = provider.GetExcuse();

        // assert
        excuse.Should().NotBeNullOrWhiteSpace("even the default provider never runs out of excuses");
    }
}
