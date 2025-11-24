using ProcrastiN8.JustBecause;
using ProcrastiN8.LazyTasks;

namespace ProcrastiN8.Tests.LazyTasks;

public class RelativisticTimeProviderTests
{
    [Fact]
    public void GetUtcNow_Returns_Earlier_Time_Near_Deadline()
    {
        // arrange
        var baseTime = new DateTimeOffset(2024, 1, 1, 12, 0, 0, TimeSpan.Zero);
        var deadline = baseTime.AddHours(1);
        var baseProvider = Substitute.For<ITimeProvider>();
        baseProvider.GetUtcNow().Returns(baseTime);
        
        var randomProvider = Substitute.For<IRandomProvider>();
        randomProvider.GetDouble().Returns(0.5); // Neutral jitter
        
        var provider = new RelativisticTimeProvider(deadline, dilationFactor: 1.0, baseTimeProvider: baseProvider, randomProvider: randomProvider);

        // act
        var result = provider.GetUtcNow();

        // assert
        result.Should().BeBefore(baseTime, "relativistic effects slow time before the deadline");
    }

    [Fact]
    public void GetUtcNow_Asymptotically_Approaches_Deadline_When_Past()
    {
        // arrange
        var deadline = new DateTimeOffset(2024, 1, 1, 12, 0, 0, TimeSpan.Zero);
        var pastDeadline = deadline.AddHours(2);
        var baseProvider = Substitute.For<ITimeProvider>();
        baseProvider.GetUtcNow().Returns(pastDeadline);
        
        var provider = new RelativisticTimeProvider(deadline, baseTimeProvider: baseProvider);

        // act
        var result = provider.GetUtcNow();

        // assert
        result.Should().BeBefore(deadline, "time never quite reaches the deadline, even when past it");
        (deadline - result).TotalSeconds.Should().BeGreaterThan(0, "always stays before deadline");
    }

    [Fact]
    public void GetUtcNow_Uses_Custom_RandomProvider_For_Jitter()
    {
        // arrange
        var baseTime = new DateTimeOffset(2024, 1, 1, 12, 0, 0, TimeSpan.Zero);
        var deadline = baseTime.AddHours(10);
        var baseProvider = Substitute.For<ITimeProvider>();
        baseProvider.GetUtcNow().Returns(baseTime);
        
        var randomProvider = Substitute.For<IRandomProvider>();
        randomProvider.GetDouble().Returns(0.5);
        
        var provider = new RelativisticTimeProvider(deadline, baseTimeProvider: baseProvider, randomProvider: randomProvider);

        // act
        var result = provider.GetUtcNow();

        // assert
        randomProvider.Received(1).GetDouble();
        result.Should().NotBe(baseTime, "quantum jitter affects the result");
    }

    [Fact]
    public void GetCurrentFlux_Returns_Severe_Dilation_Past_Deadline()
    {
        // arrange
        var deadline = new DateTimeOffset(2024, 1, 1, 12, 0, 0, TimeSpan.Zero);
        var pastDeadline = deadline.AddHours(1);
        var baseProvider = Substitute.For<ITimeProvider>();
        baseProvider.GetUtcNow().Returns(pastDeadline);
        
        var provider = new RelativisticTimeProvider(deadline, baseTimeProvider: baseProvider);

        // act
        var flux = provider.GetCurrentFlux();

        // assert
        flux.Magnitude.Should().BeLessThan(0.1, "time is severely dilated past the deadline");
        flux.Direction.Should().Be(TimeFluxDirection.Forward, "time still flows forward, slowly");
    }

    [Fact]
    public void GetCurrentFlux_Returns_Moderate_Dilation_Near_Deadline()
    {
        // arrange
        var deadline = new DateTimeOffset(2024, 1, 1, 12, 0, 0, TimeSpan.Zero);
        var nearDeadline = deadline.AddMinutes(-30);
        var baseProvider = Substitute.For<ITimeProvider>();
        baseProvider.GetUtcNow().Returns(nearDeadline);
        
        var provider = new RelativisticTimeProvider(deadline, dilationFactor: 2.0, baseTimeProvider: baseProvider);

        // act
        var flux = provider.GetCurrentFlux();

        // assert
        flux.Magnitude.Should().BeLessThan(1.0, "time is slowed near the deadline");
        flux.Magnitude.Should().BeGreaterThan(0.0, "but not completely frozen");
    }

    [Fact]
    public void Higher_DilationFactor_Causes_Stronger_Effect()
    {
        // arrange
        var baseTime = new DateTimeOffset(2024, 1, 1, 12, 0, 0, TimeSpan.Zero);
        var deadline = baseTime.AddHours(1);
        var baseProvider = Substitute.For<ITimeProvider>();
        baseProvider.GetUtcNow().Returns(baseTime);
        
        var provider1 = new RelativisticTimeProvider(deadline, dilationFactor: 1.0, baseTimeProvider: baseProvider);
        var provider2 = new RelativisticTimeProvider(deadline, dilationFactor: 5.0, baseTimeProvider: baseProvider);

        // act
        var flux1 = provider1.GetCurrentFlux();
        var flux2 = provider2.GetCurrentFlux();

        // assert
        flux2.Magnitude.Should().BeLessThan(flux1.Magnitude, "higher dilation factor causes more extreme slowdown");
    }
}
