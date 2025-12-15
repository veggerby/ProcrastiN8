using ProcrastiN8.Common;
using ProcrastiN8.NeuralExcuseLab;

namespace ProcrastiN8.Tests.NeuralExcuseLab;

public class FallbackChainExcuseProviderTests
{
    [Fact]
    public async Task GetExcuseAsync_WithSuccessfulFirstProvider_Should_ReturnExcuse()
    {
        // arrange
        var provider1 = Substitute.For<IExcuseProvider>();
        provider1.GetExcuseAsync().Returns("First excuse");
        var provider2 = Substitute.For<IExcuseProvider>();
        
        var chain = new FallbackChainExcuseProvider([provider1, provider2]);

        // act
        var excuse = await chain.GetExcuseAsync();

        // assert
        excuse.Should().Be("First excuse");
        await provider1.Received(1).GetExcuseAsync();
        await provider2.DidNotReceive().GetExcuseAsync();
    }

    [Fact]
    public async Task GetExcuseAsync_WhenFirstFails_Should_FallbackToSecond()
    {
        // arrange
        var provider1 = Substitute.For<IExcuseProvider>();
        provider1.GetExcuseAsync().Returns(Task.FromException<string>(new Exception("Provider 1 failed")));
        var provider2 = Substitute.For<IExcuseProvider>();
        provider2.GetExcuseAsync().Returns("Fallback excuse");
        
        var chain = new FallbackChainExcuseProvider([provider1, provider2]);

        // act
        var excuse = await chain.GetExcuseAsync();

        // assert
        excuse.Should().Be("Fallback excuse", "second provider should be tried");
        await provider1.Received(1).GetExcuseAsync();
        await provider2.Received(1).GetExcuseAsync();
    }

    [Fact]
    public async Task GetExcuseAsync_WhenAllFail_Should_ThrowException()
    {
        // arrange
        var provider1 = Substitute.For<IExcuseProvider>();
        provider1.GetExcuseAsync().Returns(Task.FromException<string>(new Exception("Provider 1 failed")));
        var provider2 = Substitute.For<IExcuseProvider>();
        provider2.GetExcuseAsync().Returns(Task.FromException<string>(new Exception("Provider 2 failed")));
        
        var chain = new FallbackChainExcuseProvider([provider1, provider2]);

        // act & assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => chain.GetExcuseAsync());
        exception.Message.Should().Contain("All 2 providers in the fallback chain failed");
    }

    [Fact]
    public async Task GetExcuseAsync_WithNoProviders_Should_ThrowException()
    {
        // arrange
        var chain = new FallbackChainExcuseProvider([]);

        // act & assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => chain.GetExcuseAsync());
        exception.Message.Should().Contain("No providers configured");
    }
}
