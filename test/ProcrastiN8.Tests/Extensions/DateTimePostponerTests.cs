using ProcrastiN8.Common;
using ProcrastiN8.Extensions;

namespace ProcrastiN8.Tests.Extensions;

/// <summary>
/// Unit tests for <see cref="DateTimePostponer"/> and <see cref="DefaultDateTimePostponeStrategy"/>.
/// </summary>
public class DateTimePostponerTests
{
    [Fact]
    public void Postpone_Should_Use_Default_Strategy_When_None_Provided()
    {
        // arrange
        var original = new DateTime(2025, 7, 28, 12, 0, 0);
        var postponeBy = TimeSpan.FromHours(2);

        // act
        var result = original.Postpone(postponeBy);

        // assert
        result.Should().Be(original.Add(postponeBy), "the default strategy should simply add the duration, as if that were enough");
    }

    [Fact]
    public void Postpone_Should_Use_Custom_Strategy_If_Provided()
    {
        // arrange
        var original = new DateTime(2025, 7, 28, 12, 0, 0);
        var postponeBy = TimeSpan.FromMinutes(30);
        var strategy = Substitute.For<IDateTimePostponeStrategy>();
        strategy.Postpone(original, postponeBy).Returns(original.AddDays(42));

        // act
        var result = original.Postpone(postponeBy, strategy);

        // assert
        result.Should().Be(original.AddDays(42), "the custom strategy should be honored, no matter how arbitrary");
    }

    [Fact]
    public void Postpone_Should_Log_Excuse_When_Logger_And_ExcuseProvider_Are_Provided()
    {
        // arrange
        var original = new DateTime(2025, 7, 28, 12, 0, 0);
        var postponeBy = TimeSpan.FromMinutes(15);
        var logger = Substitute.For<IProcrastiLogger>();
        var excuseProvider = Substitute.For<IExcuseProvider>();
        excuseProvider.GetExcuse().Returns("The server needed a nap.");

        // act
        var result = original.Postpone(postponeBy, null, excuseProvider, logger);

        // assert
        logger.Received().Info(Arg.Is<string>(msg => msg.Contains("The server needed a nap.")));
    }

    [Fact]
    public async Task PostponeAsync_Should_Simulate_Deliberation_And_Return_Postponed_Date()
    {
        // arrange
        var original = new DateTime(2025, 7, 28, 12, 0, 0);
        var postponeBy = TimeSpan.FromSeconds(10);
        var cts = new CancellationTokenSource();

        // act
        var result = await original.PostponeAsync(postponeBy, cancellationToken: cts.Token);

        // assert
        result.Should().Be(original.Add(postponeBy), "even async procrastination must eventually resolve");
    }

    [Fact]
    public void DefaultDateTimePostponeStrategy_Should_Add_Duration()
    {
        // arrange
        var strategy = DefaultDateTimePostponeStrategy.Instance;
        var original = new DateTime(2025, 7, 28, 12, 0, 0);
        var postponeBy = TimeSpan.FromDays(1);

        // act
        var result = strategy.Postpone(original, postponeBy);

        // assert
        result.Should().Be(original.AddDays(1), "the default strategy is as literal as it is uninspired");
    }

    [Fact]
    public void Postpone_Should_Not_Log_If_Logger_Or_ExcuseProvider_Is_Null()
    {
        // arrange
        var original = new DateTime(2025, 7, 28, 12, 0, 0);
        var postponeBy = TimeSpan.FromMinutes(5);
        var logger = Substitute.For<IProcrastiLogger>();
        var excuseProvider = Substitute.For<IExcuseProvider>();

        // act
        original.Postpone(postponeBy, null, null, logger); // No excuseProvider
        original.Postpone(postponeBy, null, excuseProvider, null); // No logger

        // assert
        logger.DidNotReceive().Info(Arg.Any<string>());
    }

    [Fact]
    public void Postpone_Should_Handle_Zero_Duration()
    {
        // arrange
        var original = new DateTime(2025, 7, 28, 12, 0, 0);
        var postponeBy = TimeSpan.Zero;

        // act
        var result = original.Postpone(postponeBy);

        // assert
        result.Should().Be(original, "no time was wasted, which is highly irregular");
    }
}
