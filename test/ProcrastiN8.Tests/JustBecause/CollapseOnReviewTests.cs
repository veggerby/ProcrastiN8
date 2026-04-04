using ProcrastiN8.JustBecause;

namespace ProcrastiN8.Tests.JustBecause;

public class CollapseOnReviewTests
{
    [Fact]
    public void Resolve_InTestContext_ReturnsReviewValue()
    {
        // arrange — we are running inside xUnit, which is a review context by definition
        var value = new CollapseOnReview<string>(
            productionValue: "what production sees",
            reviewValue: "what reviewers see");

        // act — this method runs inside the xUnit test framework
        var result = value.Resolve();

        // assert — the waveform collapses to the reviewed value because xUnit is on the stack
        result.Should().Be("what reviewers see",
            "the test harness is always watching — the waveform collapses under observation");
    }

    [Fact]
    public void IsUnderReview_InTestContext_ReturnsTrue()
    {
        // arrange
        var value = new CollapseOnReview<int>(0, 1);

        // act
        var underReview = value.IsUnderReview;

        // assert — the test framework is a known review indicator
        underReview.Should().BeTrue("xUnit's presence on the call stack is indistinguishable from a code reviewer");
    }

    [Fact]
    public void Properties_ExposeProductionAndReviewValues()
    {
        // arrange
        var value = new CollapseOnReview<int>(productionValue: 100, reviewValue: 42);

        // assert — both values are accessible regardless of observation state
        value.ProductionValue.Should().Be(100, "the production value is always technically available");
        value.ReviewValue.Should().Be(42, "the review value is the diplomatic face presented to observers");
    }
}
