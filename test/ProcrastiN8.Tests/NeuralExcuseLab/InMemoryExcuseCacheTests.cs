using ProcrastiN8.NeuralExcuseLab;

namespace ProcrastiN8.Tests.NeuralExcuseLab;

public class InMemoryExcuseCacheTests
{
    [Fact]
    public void TryGet_OnEmptyCache_Should_ReturnFalse()
    {
        // arrange
        var cache = new InMemoryExcuseCache();

        // act
        var result = cache.TryGet("test-prompt", out var excuse);

        // assert
        result.Should().BeFalse("cache is empty");
        excuse.Should().BeNull();
    }

    [Fact]
    public void Set_And_TryGet_Should_CacheExcuse()
    {
        // arrange
        var cache = new InMemoryExcuseCache();
        var prompt = "test-prompt";
        var excuseText = "I was debugging quantum entanglements";

        // act
        cache.Set(prompt, excuseText);
        var result = cache.TryGet(prompt, out var excuse);

        // assert
        result.Should().BeTrue("excuse was just cached");
        excuse.Should().Be(excuseText);
    }

    [Fact]
    public void Clear_Should_RemoveAllCachedExcuses()
    {
        // arrange
        var cache = new InMemoryExcuseCache();
        cache.Set("prompt1", "excuse1");
        cache.Set("prompt2", "excuse2");

        // act
        cache.Clear();
        var result = cache.TryGet("prompt1", out _);

        // assert
        result.Should().BeFalse("cache was cleared");
    }

    [Fact]
    public void GetStatistics_Should_TrackHitsAndMisses()
    {
        // arrange
        var cache = new InMemoryExcuseCache();
        cache.Set("prompt1", "excuse1");

        // act
        cache.TryGet("prompt1", out _); // hit
        cache.TryGet("prompt2", out _); // miss
        cache.TryGet("prompt1", out _); // hit
        var stats = cache.GetStatistics();

        // assert
        stats["cache_hits"].Should().Be(2);
        stats["cache_misses"].Should().Be(1);
        stats["total_requests"].Should().Be(3);
    }

    [Fact]
    public void GetStatistics_Should_CalculateHitRate()
    {
        // arrange
        var cache = new InMemoryExcuseCache();
        cache.Set("prompt1", "excuse1");
        cache.TryGet("prompt1", out _); // hit
        cache.TryGet("prompt2", out _); // miss

        // act
        var stats = cache.GetStatistics();

        // assert
        stats["hit_rate"].Should().Be(0.5, "1 hit out of 2 requests");
    }
}
