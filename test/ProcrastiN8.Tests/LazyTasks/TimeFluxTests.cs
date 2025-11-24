using ProcrastiN8.LazyTasks;

namespace ProcrastiN8.Tests.LazyTasks;

public class TimeFluxTests
{
    [Fact]
    public void Normal_TimeFlux_Has_Unit_Magnitude()
    {
        // arrange & act
        var flux = TimeFlux.Normal;

        // assert
        flux.Magnitude.Should().Be(1.0, "normal time flow has no distortion");
        flux.Direction.Should().Be(TimeFluxDirection.Forward, "normal time flows forward");
        flux.IsParadoxical().Should().BeFalse("normal time flow does not violate causality");
    }

    [Fact]
    public void Frozen_TimeFlux_Has_Zero_Magnitude()
    {
        // arrange & act
        var flux = TimeFlux.Frozen;

        // assert
        flux.Magnitude.Should().Be(0.0, "frozen time has no progression");
        flux.Direction.Should().Be(TimeFluxDirection.Forward, "frozen time still faces forward, uselessly");
    }

    [Fact]
    public void Apply_Scales_TimeSpan_By_Magnitude()
    {
        // arrange
        var flux = new TimeFlux(2.0, TimeFluxDirection.Forward);
        var duration = TimeSpan.FromHours(1);

        // act
        var result = flux.Apply(duration);

        // assert
        result.Should().Be(TimeSpan.FromHours(2), "time flux doubles the perceived duration");
    }

    [Fact]
    public void Apply_Reverses_TimeSpan_When_Backward()
    {
        // arrange
        var flux = new TimeFlux(1.5, TimeFluxDirection.Backward);
        var duration = TimeSpan.FromHours(2);

        // act
        var result = flux.Apply(duration);

        // assert
        result.Should().Be(TimeSpan.FromHours(-3), "backward time flux creates negative duration");
    }

    [Fact]
    public void IsParadoxical_Returns_True_For_Backward_Direction()
    {
        // arrange
        var flux = new TimeFlux(1.0, TimeFluxDirection.Backward);

        // act & assert
        flux.IsParadoxical().Should().BeTrue("backward time is inherently paradoxical");
    }

    [Fact]
    public void IsParadoxical_Returns_True_For_Negative_Magnitude()
    {
        // arrange
        var flux = new TimeFlux(-0.5, TimeFluxDirection.Forward);

        // act & assert
        flux.IsParadoxical().Should().BeTrue("negative magnitude violates causality");
    }

    [Fact]
    public void Equality_Works_Correctly()
    {
        // arrange
        var flux1 = new TimeFlux(1.5, TimeFluxDirection.Forward);
        var flux2 = new TimeFlux(1.5, TimeFluxDirection.Forward);
        var flux3 = new TimeFlux(2.0, TimeFluxDirection.Forward);

        // act & assert
        flux1.Should().Be(flux2, "identical fluxes are equal");
        flux1.Should().NotBe(flux3, "different magnitudes are not equal");
        (flux1 == flux2).Should().BeTrue("equality operator works");
        (flux1 != flux3).Should().BeTrue("inequality operator works");
    }

    [Fact]
    public void ToString_Returns_Readable_Format()
    {
        // arrange
        var flux = new TimeFlux(2.5, TimeFluxDirection.Backward);

        // act
        var result = flux.ToString();

        // assert
        result.Should().Contain("2.50", "magnitude is included");
        result.Should().Contain("Backward", "direction is included");
    }
}
