using ProcrastiN8.JustBecause;
using ProcrastiN8.LazyTasks;

namespace ProcrastiN8.Tests.LazyTasks;

public class MercuryRetrogradeProviderTests
{
    [Fact]
    public void GetUtcNow_Returns_Real_Time_When_Not_Retrograde()
    {
        // arrange
        var baseTime = new DateTimeOffset(2024, 1, 1, 12, 0, 0, TimeSpan.Zero);
        var baseProvider = Substitute.For<ITimeProvider>();
        baseProvider.GetUtcNow().Returns(baseTime);
        
        var randomProvider = Substitute.For<IRandomProvider>();
        randomProvider.GetDouble().Returns(0.9); // Above 30% threshold, no retrograde
        
        var provider = new MercuryRetrogradeProvider(
            retrogradeProbability: 0.3,
            baseTimeProvider: baseProvider,
            randomProvider: randomProvider);

        // act
        provider.GetUtcNow(); // First call initializes
        var result = provider.GetUtcNow();

        // assert
        result.Should().Be(baseTime, "no retrograde means real time is returned");
    }

    [Fact]
    public void GetUtcNow_Rewinds_Time_During_Retrograde()
    {
        // arrange
        var baseTime = new DateTimeOffset(2024, 1, 1, 12, 0, 0, TimeSpan.Zero);
        var baseProvider = Substitute.For<ITimeProvider>();
        baseProvider.GetUtcNow().Returns(baseTime);
        
        var randomProvider = Substitute.For<IRandomProvider>();
        randomProvider.GetDouble().Returns(0.1, 0.5); // First triggers retrograde, second for rewind amount
        
        var provider = new MercuryRetrogradeProvider(
            retrogradeProbability: 0.3,
            maxRewindDuration: TimeSpan.FromHours(2),
            baseTimeProvider: baseProvider,
            randomProvider: randomProvider);

        // act
        provider.GetUtcNow(); // First call initializes
        var result = provider.GetUtcNow();

        // assert
        result.Should().BeBefore(baseTime, "Mercury retrograde causes time to rewind");
    }

    [Fact]
    public void Constructor_Throws_For_Invalid_Probability()
    {
        // arrange & act
        Action act1 = () => new MercuryRetrogradeProvider(retrogradeProbability: -0.1);
        Action act2 = () => new MercuryRetrogradeProvider(retrogradeProbability: 1.5);

        // assert
        act1.Should().Throw<ArgumentOutOfRangeException>("negative probability is invalid");
        act2.Should().Throw<ArgumentOutOfRangeException>("probability above 1.0 is invalid");
    }

    [Fact]
    public void ConsecutiveRewinds_Increments_During_Retrograde()
    {
        // arrange
        var baseTime = new DateTimeOffset(2024, 1, 1, 12, 0, 0, TimeSpan.Zero);
        var baseProvider = Substitute.For<ITimeProvider>();
        baseProvider.GetUtcNow().Returns(baseTime);
        
        var randomProvider = Substitute.For<IRandomProvider>();
        randomProvider.GetDouble().Returns(0.1, 0.5, 0.1, 0.5); // Alternating retrograde triggers
        
        var provider = new MercuryRetrogradeProvider(
            retrogradeProbability: 0.3,
            baseTimeProvider: baseProvider,
            randomProvider: randomProvider);

        // act
        provider.GetUtcNow(); // Initialize
        provider.GetUtcNow(); // First retrograde
        var count = provider.ConsecutiveRewinds;

        // assert
        count.Should().Be(1, "one retrograde event has occurred");
    }

    [Fact]
    public void IsCurrentlyRetrograde_Returns_True_After_Rewind()
    {
        // arrange
        var baseTime = new DateTimeOffset(2024, 1, 1, 12, 0, 0, TimeSpan.Zero);
        var baseProvider = Substitute.For<ITimeProvider>();
        baseProvider.GetUtcNow().Returns(baseTime);
        
        var randomProvider = Substitute.For<IRandomProvider>();
        randomProvider.GetDouble().Returns(0.1, 0.5); // Trigger retrograde
        
        var provider = new MercuryRetrogradeProvider(
            retrogradeProbability: 0.3,
            baseTimeProvider: baseProvider,
            randomProvider: randomProvider);

        // act
        provider.GetUtcNow(); // Initialize
        provider.GetUtcNow(); // Trigger retrograde

        // assert
        provider.IsCurrentlyRetrograde().Should().BeTrue("Mercury is in retrograde");
    }

    [Fact]
    public void Throws_TemporalWhiplashException_After_Too_Many_Consecutive_Rewinds()
    {
        // arrange
        var baseTime = new DateTimeOffset(2024, 1, 1, 12, 0, 0, TimeSpan.Zero);
        var baseProvider = Substitute.For<ITimeProvider>();
        baseProvider.GetUtcNow().Returns(baseTime);
        
        var randomProvider = Substitute.For<IRandomProvider>();
        // All calls return low value to ensure retrograde always triggers
        randomProvider.GetDouble().Returns(0.1);
        
        var provider = new MercuryRetrogradeProvider(
            retrogradeProbability: 0.3,  // 0.1 < 0.3, so always triggers
            baseTimeProvider: baseProvider,
            randomProvider: randomProvider);

        // act
        provider.GetUtcNow(); // Initialize (doesn't check retrograde)
        Action act = () =>
        {
            // Need 6 consecutive rewinds to trigger whiplash (> 5)
            for (int i = 0; i < 7; i++)
            {
                provider.GetUtcNow();
            }
        };

        // assert
        act.Should().Throw<TemporalWhiplashException>("too many consecutive rewinds causes whiplash");
    }

    [Fact]
    public void ResetRetrogradeState_Clears_Consecutive_Rewinds()
    {
        // arrange
        var baseTime = new DateTimeOffset(2024, 1, 1, 12, 0, 0, TimeSpan.Zero);
        var baseProvider = Substitute.For<ITimeProvider>();
        baseProvider.GetUtcNow().Returns(baseTime);
        
        var randomProvider = Substitute.For<IRandomProvider>();
        randomProvider.GetDouble().Returns(0.1, 0.5); // Trigger retrograde
        
        var provider = new MercuryRetrogradeProvider(
            retrogradeProbability: 0.3,
            baseTimeProvider: baseProvider,
            randomProvider: randomProvider);

        // act
        provider.GetUtcNow(); // Initialize
        provider.GetUtcNow(); // Trigger retrograde
        provider.ResetRetrogradeState();

        // assert
        provider.ConsecutiveRewinds.Should().Be(0, "reset clears the counter");
        provider.IsCurrentlyRetrograde().Should().BeFalse("reset clears retrograde state");
    }

    [Fact]
    public void ConsecutiveRewinds_Resets_When_Normal_Time_Resumes()
    {
        // arrange
        var baseTime = new DateTimeOffset(2024, 1, 1, 12, 0, 0, TimeSpan.Zero);
        var baseProvider = Substitute.For<ITimeProvider>();
        baseProvider.GetUtcNow().Returns(baseTime);
        
        var randomProvider = Substitute.For<IRandomProvider>();
        // First retrograde, then normal
        randomProvider.GetDouble().Returns(0.1, 0.5, 0.9);
        
        var provider = new MercuryRetrogradeProvider(
            retrogradeProbability: 0.3,
            baseTimeProvider: baseProvider,
            randomProvider: randomProvider);

        // act
        provider.GetUtcNow(); // Initialize
        provider.GetUtcNow(); // Retrograde
        provider.GetUtcNow(); // Normal

        // assert
        provider.ConsecutiveRewinds.Should().Be(0, "normal time resets the counter");
    }
}
