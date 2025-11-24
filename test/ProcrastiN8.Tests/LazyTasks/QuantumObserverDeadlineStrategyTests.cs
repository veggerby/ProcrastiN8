using ProcrastiN8.JustBecause;
using ProcrastiN8.LazyTasks;

namespace ProcrastiN8.Tests.LazyTasks;

public class QuantumObserverDeadlineStrategyTests
{
    [Fact]
    public void DistortDeadline_Shifts_Forward_On_First_Observation()
    {
        // arrange
        var originalDeadline = new DateTimeOffset(2024, 1, 1, 12, 0, 0, TimeSpan.Zero);
        var currentTime = new DateTimeOffset(2024, 1, 1, 10, 0, 0, TimeSpan.Zero);
        var strategy = new QuantumObserverDeadlineStrategy(shiftPerObservation: TimeSpan.FromHours(1));

        // act
        var distorted = strategy.DistortDeadline(originalDeadline, currentTime, observationCount: 1);

        // assert
        distorted.Should().BeAfter(originalDeadline, "observing the deadline pushes it away");
    }

    [Fact]
    public void DistortDeadline_Shifts_More_With_Multiple_Observations()
    {
        // arrange
        var originalDeadline = new DateTimeOffset(2024, 1, 1, 12, 0, 0, TimeSpan.Zero);
        var currentTime = new DateTimeOffset(2024, 1, 1, 10, 0, 0, TimeSpan.Zero);
        var strategy = new QuantumObserverDeadlineStrategy(shiftPerObservation: TimeSpan.FromHours(1));

        // act
        var distorted1 = strategy.DistortDeadline(originalDeadline, currentTime, observationCount: 1);
        var distorted5 = strategy.DistortDeadline(originalDeadline, currentTime, observationCount: 5);

        // assert
        distorted5.Should().BeAfter(distorted1, "more observations push the deadline further away");
    }

    [Fact]
    public void DistortDeadline_Collapses_After_Paradox_Threshold()
    {
        // arrange
        var originalDeadline = new DateTimeOffset(2024, 1, 1, 12, 0, 0, TimeSpan.Zero);
        var currentTime = new DateTimeOffset(2024, 1, 1, 10, 0, 0, TimeSpan.Zero);
        var randomProvider = Substitute.For<IRandomProvider>();
        randomProvider.GetDouble().Returns(0.5);
        
        var strategy = new QuantumObserverDeadlineStrategy(
            shiftPerObservation: TimeSpan.FromHours(1),
            paradoxThreshold: 10,
            randomProvider: randomProvider);

        // act
        var distorted = strategy.DistortDeadline(originalDeadline, currentTime, observationCount: 15);

        // assert
        // After threshold, should collapse back near original (with jitter)
        var shift = Math.Abs((distorted - originalDeadline).TotalHours);
        shift.Should().BeLessThan(1.0, "deadline collapses back to near original after paradox threshold");
    }

    [Fact]
    public void DistortDeadline_Uses_Custom_RandomProvider_For_Jitter()
    {
        // arrange
        var originalDeadline = new DateTimeOffset(2024, 1, 1, 12, 0, 0, TimeSpan.Zero);
        var currentTime = new DateTimeOffset(2024, 1, 1, 10, 0, 0, TimeSpan.Zero);
        var randomProvider = Substitute.For<IRandomProvider>();
        randomProvider.GetDouble().Returns(0.5);
        
        var strategy = new QuantumObserverDeadlineStrategy(randomProvider: randomProvider);

        // act
        strategy.DistortDeadline(originalDeadline, currentTime, observationCount: 1);

        // assert
        randomProvider.Received().GetDouble();
    }

    [Fact]
    public void IsParadoxical_Returns_False_For_Normal_Shift()
    {
        // arrange
        var originalDeadline = new DateTimeOffset(2024, 1, 1, 12, 0, 0, TimeSpan.Zero);
        var distortedDeadline = originalDeadline.AddHours(5);
        var strategy = new QuantumObserverDeadlineStrategy();

        // act
        var isParadoxical = strategy.IsParadoxical(originalDeadline, distortedDeadline);

        // assert
        isParadoxical.Should().BeFalse("moderate shift is not paradoxical");
    }

    [Fact]
    public void IsParadoxical_Returns_True_When_Deadline_Moves_Backward()
    {
        // arrange
        var originalDeadline = new DateTimeOffset(2024, 1, 1, 12, 0, 0, TimeSpan.Zero);
        var distortedDeadline = originalDeadline.AddHours(-2);
        var strategy = new QuantumObserverDeadlineStrategy();

        // act
        var isParadoxical = strategy.IsParadoxical(originalDeadline, distortedDeadline);

        // assert
        isParadoxical.Should().BeTrue("deadline moving backward violates causality");
    }

    [Fact]
    public void IsParadoxical_Returns_True_When_Shift_Exceeds_Year()
    {
        // arrange
        var originalDeadline = new DateTimeOffset(2024, 1, 1, 12, 0, 0, TimeSpan.Zero);
        var distortedDeadline = originalDeadline.AddDays(400);
        var strategy = new QuantumObserverDeadlineStrategy();

        // act
        var isParadoxical = strategy.IsParadoxical(originalDeadline, distortedDeadline);

        // assert
        isParadoxical.Should().BeTrue("deadline shifting over a year is paradoxical");
    }

    [Fact]
    public void Custom_ShiftPerObservation_Is_Respected()
    {
        // arrange
        var originalDeadline = new DateTimeOffset(2024, 1, 1, 12, 0, 0, TimeSpan.Zero);
        var currentTime = new DateTimeOffset(2024, 1, 1, 10, 0, 0, TimeSpan.Zero);
        var strategy = new QuantumObserverDeadlineStrategy(shiftPerObservation: TimeSpan.FromMinutes(30));

        // act
        var distorted = strategy.DistortDeadline(originalDeadline, currentTime, observationCount: 2);

        // assert
        // Expected shift: 2 observations * 30 minutes = 60 minutes, plus jitter
        var shift = (distorted - originalDeadline).TotalMinutes;
        shift.Should().BeGreaterThan(45, "shift is approximately 2 * 30 minutes");
        shift.Should().BeLessThan(75, "but jitter keeps it reasonable");
    }

    [Fact]
    public void Zero_Observations_Returns_Original_Deadline()
    {
        // arrange
        var originalDeadline = new DateTimeOffset(2024, 1, 1, 12, 0, 0, TimeSpan.Zero);
        var currentTime = new DateTimeOffset(2024, 1, 1, 10, 0, 0, TimeSpan.Zero);
        var strategy = new QuantumObserverDeadlineStrategy();

        // act
        var distorted = strategy.DistortDeadline(originalDeadline, currentTime, observationCount: 0);

        // assert
        // With zero observations, shift should be minimal (just jitter)
        var shift = Math.Abs((distorted - originalDeadline).TotalMinutes);
        shift.Should().BeLessThan(20, "zero observations means near-original deadline");
    }
}
