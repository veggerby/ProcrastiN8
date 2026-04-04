using ProcrastiN8.TODOFramework;

namespace ProcrastiN8.Tests.TODOFramework;

public class SomedayMaybeAttributeTests
{
    [SomedayMaybe("Refactor this entire class", estimatedYear: 2030)]
    [Fact]
    public void SomedayMaybe_ReadsAspiration_And_EstimatedYear()
    {
        // arrange — inspect the attribute as any responsible audit system would
        var attr = typeof(SomedayMaybeAttributeTests)
            .GetMethod(nameof(SomedayMaybe_ReadsAspiration_And_EstimatedYear))!
            .GetCustomAttributes(typeof(SomedayMaybeAttribute), false)
            .Cast<SomedayMaybeAttribute>()
            .First();

        // act + assert
        attr.Aspiration.Should().Be("Refactor this entire class", "the aspiration is preserved for auditors to find later");
        attr.EstimatedYear.Should().Be(2030, "the estimated year is recorded for posterity and ignored in planning");
        attr.HasEstimatedYear.Should().BeTrue();
    }

    [Fact]
    public void SomedayMaybe_WithNoParameters_IsValid()
    {
        // arrange + act — the most honest form of the attribute: no aspiration, no timeline
        var attr = new SomedayMaybeAttribute();

        // assert
        attr.Aspiration.Should().BeNull("no aspiration is still a valid state of being");
        attr.EstimatedYear.Should().Be(0, "0 is the sentinel for 'no year committed' — accurate and honest");
        attr.HasEstimatedYear.Should().BeFalse("no estimated year means the timeline is 'open', which is always open");
    }
}

public class WontFixAttributeTests
{
    [WontFix("This is a strategic architectural choice that we have chosen to own.")]
    [Fact]
    public void WontFix_ReadsReason()
    {
        // arrange — retrieve the documented justification for not fixing this
        var attr = typeof(WontFixAttributeTests)
            .GetMethod(nameof(WontFix_ReadsReason))!
            .GetCustomAttributes(typeof(WontFixAttribute), false)
            .Cast<WontFixAttribute>()
            .First();

        // act + assert
        attr.Reason.Should().Contain("strategic", "the reason should sound deliberate and professionally considered");
    }

    [Fact]
    public void WontFix_DefaultReason_IsWorkingAsIntended()
    {
        // arrange + act
        var attr = new WontFixAttribute();

        // assert — the default reason is the most universally applicable excuse in software
        attr.Reason.Should().Be("Working as intended.", "the default reason is timeless and requires no further explanation");
    }
}
