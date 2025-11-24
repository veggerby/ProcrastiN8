using ProcrastiN8.LazyTasks;

namespace ProcrastiN8.Tests.LazyTasks;

public class NostalgicTimeProviderTests
{
    [Fact]
    public void GetUtcNow_Returns_Time_In_Nostalgic_Year()
    {
        // arrange
        var provider = new NostalgicTimeProvider(2019);

        // act
        var result = provider.GetUtcNow();

        // assert
        result.Year.Should().Be(2019, "the provider is stuck in 2019");
    }

    [Fact]
    public void GetUtcNow_Uses_Default_2019_When_Not_Specified()
    {
        // arrange
        var provider = new NostalgicTimeProvider();

        // act
        var result = provider.GetUtcNow();

        // assert
        result.Year.Should().Be(2019, "defaults to 2019, the last good year");
    }

    [Fact]
    public void GetUtcNow_Maintains_Time_Progression_Within_Year()
    {
        // arrange
        var anchorTime = new DateTimeOffset(2024, 11, 24, 10, 0, 0, TimeSpan.Zero);
        var laterTime = anchorTime.AddMinutes(30);
        
        var baseProvider = Substitute.For<ITimeProvider>();
        baseProvider.GetUtcNow().Returns(anchorTime);
        
        var provider = new NostalgicTimeProvider(2019, baseProvider);

        // act
        var time1 = provider.GetUtcNow();
        
        // Now update the base provider to return a later time
        baseProvider.GetUtcNow().Returns(laterTime);
        var time2 = provider.GetUtcNow();

        // assert
        time2.Should().BeAfter(time1, "time still progresses, just in the wrong year");
        (time2 - time1).Should().Be(TimeSpan.FromMinutes(30), "elapsed time is preserved");
    }

    [Fact]
    public void GetUtcNow_Loops_When_Exceeding_Year_End()
    {
        // arrange
        var baseProvider = Substitute.For<ITimeProvider>();
        var yearStart = new DateTimeOffset(2024, 1, 1, 0, 0, 0, TimeSpan.Zero);
        var wayPastYear = yearStart.AddDays(400);
        baseProvider.GetUtcNow().Returns(wayPastYear);
        
        var provider = new NostalgicTimeProvider(2019, baseProvider);

        // act
        var result = provider.GetUtcNow();

        // assert
        result.Year.Should().Be(2019, "loops back to nostalgic year");
        result.Month.Should().BeLessThanOrEqualTo(12, "stays within valid months");
    }

    [Fact]
    public void Constructor_Throws_For_Invalid_Year()
    {
        // arrange & act
        Action act1 = () => new NostalgicTimeProvider(1899);
        Action act2 = () => new NostalgicTimeProvider(2101);

        // assert
        act1.Should().Throw<ArgumentOutOfRangeException>("1899 is too far back");
        act2.Should().Throw<ArgumentOutOfRangeException>("2101 is too far forward");
    }

    [Fact]
    public void NostalgicYear_Property_Returns_Configured_Year()
    {
        // arrange
        var provider = new NostalgicTimeProvider(2010);

        // act & assert
        provider.NostalgicYear.Should().Be(2010, "the year we're stuck in");
    }

    [Fact]
    public void HasLooped_Returns_False_Before_Year_End()
    {
        // arrange
        var baseProvider = Substitute.For<ITimeProvider>();
        var anchorTime = new DateTimeOffset(2024, 1, 1, 0, 0, 0, TimeSpan.Zero);
        var withinYear = anchorTime.AddDays(100);
        baseProvider.GetUtcNow().Returns(anchorTime, withinYear);
        
        var provider = new NostalgicTimeProvider(2019, baseProvider);
        provider.GetUtcNow(); // Initialize

        // act
        var hasLooped = provider.HasLooped();

        // assert
        hasLooped.Should().BeFalse("haven't exceeded the year yet");
    }

    [Fact]
    public void HasLooped_Returns_True_After_Year_Exceeded()
    {
        // arrange
        var baseProvider = Substitute.For<ITimeProvider>();
        var anchorTime = new DateTimeOffset(2024, 1, 1, 0, 0, 0, TimeSpan.Zero);
        var pastYear = anchorTime.AddDays(400);
        baseProvider.GetUtcNow().Returns(anchorTime, pastYear);
        
        var provider = new NostalgicTimeProvider(2019, baseProvider);
        provider.GetUtcNow(); // Initialize

        // act
        var hasLooped = provider.HasLooped();

        // assert
        hasLooped.Should().BeTrue("time has looped at least once");
    }

    [Fact]
    public void Multiple_Calls_Show_Consistent_Nostalgic_Behavior()
    {
        // arrange
        var provider = new NostalgicTimeProvider(2015);

        // act
        var results = Enumerable.Range(0, 10)
            .Select(_ => provider.GetUtcNow())
            .ToList();

        // assert
        results.Should().AllSatisfy(r => r.Year.Should().Be(2015), "all timestamps remain in 2015");
        results.Should().BeInAscendingOrder("time still progresses monotonically within the year");
    }
}
