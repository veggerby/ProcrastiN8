using ProcrastiN8.LazyTasks;
using ProcrastiN8.JustBecause;

namespace ProcrastiN8.Tests.LazyTasks;

/// <summary>
/// Edge and error path coverage for <see cref="DefaultDelayStrategy"/>.
/// </summary>
public class DefaultDelayStrategyEdgeTests
{
    [Fact]
    public async Task BeforeCallback_Can_Skip_Delay()
    {
        // arrange
        var delayProvider = Substitute.For<IDelayProvider>();
        var strategy = new DefaultDelayStrategy(TimeSpan.FromMilliseconds(10), TimeSpan.FromMilliseconds(10), delayProvider: delayProvider);

        // act
        await strategy.DelayAsync(beforeCallback: _ => false);

        // assert
        await delayProvider.DidNotReceive().DelayAsync(Arg.Any<TimeSpan>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Range_Swap_When_Min_Greater_Than_Max()
    {
        // arrange
        var delayProvider = Substitute.For<IDelayProvider>();
        var strategy = new DefaultDelayStrategy(TimeSpan.FromMilliseconds(30), TimeSpan.FromMilliseconds(10), delayProvider: delayProvider, randomProvider: Substitute.For<IRandomProvider>());

        // act
        await strategy.DelayAsync();

        // assert
        await delayProvider.Received(1).DelayAsync(TimeSpan.FromMilliseconds(10), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Fixed_Delay_Path_When_Min_Equals_Max()
    {
        // arrange
        var delayProvider = Substitute.For<IDelayProvider>();
        var strategy = new DefaultDelayStrategy(TimeSpan.FromMilliseconds(25), TimeSpan.FromMilliseconds(25), delayProvider: delayProvider, randomProvider: Substitute.For<IRandomProvider>());

        // act
        await strategy.DelayAsync();

        // assert
        await delayProvider.Received(1).DelayAsync(TimeSpan.FromMilliseconds(25), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Negative_Delay_Throws()
    {
        var strategy = new DefaultDelayStrategy();
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => strategy.DelayAsync(TimeSpan.FromMilliseconds(-1), TimeSpan.FromMilliseconds(5)));
    }
}
