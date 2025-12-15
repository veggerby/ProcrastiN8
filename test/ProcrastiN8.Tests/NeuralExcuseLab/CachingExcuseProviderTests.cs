using ProcrastiN8.Common;
using ProcrastiN8.NeuralExcuseLab;

namespace ProcrastiN8.Tests.NeuralExcuseLab;

public class CachingExcuseProviderTests
{
    [Fact]
    public async Task GetExcuseAsync_OnCacheMiss_Should_CallInnerProvider()
    {
        // arrange
        var innerProvider = Substitute.For<IExcuseProvider>();
        innerProvider.GetExcuseAsync().Returns("Fresh excuse");
        var cache = new InMemoryExcuseCache();
        var cachingProvider = new CachingExcuseProvider(innerProvider, cache);

        // act
        var excuse = await cachingProvider.GetExcuseAsync();

        // assert
        excuse.Should().Be("Fresh excuse");
        await innerProvider.Received(1).GetExcuseAsync();
    }

    [Fact]
    public async Task GetExcuseAsync_OnCacheHit_Should_NotCallInnerProvider()
    {
        // arrange
        var innerProvider = Substitute.For<IExcuseProvider>();
        innerProvider.GetExcuseAsync().Returns("Fresh excuse");
        var cache = new InMemoryExcuseCache();
        var cachingProvider = new CachingExcuseProvider(innerProvider, cache);

        // act
        await cachingProvider.GetExcuseAsync(); // First call - cache miss
        var excuse = await cachingProvider.GetExcuseAsync(); // Second call - cache hit

        // assert
        excuse.Should().Be("Fresh excuse", "cached excuse should be returned");
        await innerProvider.Received(1).GetExcuseAsync();
    }

    [Fact]
    public async Task GetExcuseAsync_Should_CacheResult()
    {
        // arrange
        var innerProvider = Substitute.For<IExcuseProvider>();
        innerProvider.GetExcuseAsync().Returns("Fresh excuse");
        var cache = new InMemoryExcuseCache();
        var cachingProvider = new CachingExcuseProvider(innerProvider, cache);

        // act
        await cachingProvider.GetExcuseAsync();
        var stats = cachingProvider.GetCacheStatistics();

        // assert
        stats["cached_entries"].Should().Be(1, "excuse should be cached");
    }
}
