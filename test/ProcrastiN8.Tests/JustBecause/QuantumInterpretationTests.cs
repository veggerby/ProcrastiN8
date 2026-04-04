using ProcrastiN8.JustBecause;
using ProcrastiN8.JustBecause.Interpretations;

namespace ProcrastiN8.Tests.JustBecause;

public class QuantumInterpretationTests
{
    // ──────────────────────────────────────────────────────────────────────────
    // Registry
    // ──────────────────────────────────────────────────────────────────────────

    [Fact]
    public void QuantumInterpretations_All_ContainsSixInterpretations()
    {
        // arrange + act — the universe currently ships six interpretations
        var all = QuantumInterpretations.All;

        // assert
        all.Should().HaveCount(6, "there are exactly six well-known quantum interpretations in this library, none of them testable by experiment");
    }

    [Fact]
    public void QuantumInterpretations_ByName_ReturnsMatchingInterpretation()
    {
        // arrange + act
        var found = QuantumInterpretations.ByName("Copenhagen");

        // assert
        found.Name.Should().Be("Copenhagen", "name lookup is case-insensitive and the Copenhagen interpretation is the most popular");
    }

    [Fact]
    public void QuantumInterpretations_ByName_UnknownName_ReturnsCopenhagen()
    {
        // arrange + act
        var found = QuantumInterpretations.ByName("FlatEarth");

        // assert
        found.Should().BeSameAs(QuantumInterpretations.Copenhagen, "defaulting to Copenhagen is itself a very Copenhagen move");
    }

    // ──────────────────────────────────────────────────────────────────────────
    // Interpretation contract: ObservationAffectsOutcome
    // ──────────────────────────────────────────────────────────────────────────

    [Fact]
    public void Copenhagen_ObservationAffectsOutcome_IsTrue()
    {
        // arrange + act + assert
        QuantumInterpretations.Copenhagen.ObservationAffectsOutcome
            .Should().BeTrue("the Copenhagen interpretation collapses the wavefunction when observed");
    }

    [Fact]
    public void ManyWorlds_ObservationAffectsOutcome_IsFalse()
    {
        // arrange + act + assert
        QuantumInterpretations.ManyWorlds.ObservationAffectsOutcome
            .Should().BeFalse("in Many-Worlds, all outcomes already exist; observation merely selects a branch");
    }

    [Fact]
    public void PilotWave_ObservationAffectsOutcome_IsFalse()
    {
        // arrange + act + assert
        QuantumInterpretations.PilotWave.ObservationAffectsOutcome
            .Should().BeFalse("in the Pilot Wave interpretation, the trajectory is fixed; observation reveals it, not changes it");
    }

    // ──────────────────────────────────────────────────────────────────────────
    // Interpretation contract: ParallelTimelinesAreReal
    // ──────────────────────────────────────────────────────────────────────────

    [Fact]
    public void ManyWorlds_ParallelTimelinesAreReal_IsTrue()
    {
        // arrange + act + assert
        QuantumInterpretations.ManyWorlds.ParallelTimelinesAreReal
            .Should().BeTrue("in Many-Worlds, every branch is equally real; cancelling them is philosophically incorrect");
    }

    [Fact]
    public void Copenhagen_ParallelTimelinesAreReal_IsFalse()
    {
        // arrange + act + assert
        QuantumInterpretations.Copenhagen.ParallelTimelinesAreReal
            .Should().BeFalse("Copenhagen allows only one outcome; the rest collapse into non-existence");
    }

    // ──────────────────────────────────────────────────────────────────────────
    // Interpretation contract: InterpretProbability
    // ──────────────────────────────────────────────────────────────────────────

    [Fact]
    public void Copenhagen_InterpretProbability_ReturnsStatedProbability()
    {
        // arrange — Copenhagen applies the standard Born rule without adjustment
        var interpretation = QuantumInterpretations.Copenhagen;

        // act
        var result = interpretation.InterpretProbability(0.3, 0.7);

        // assert
        result.Should().Be(0.7, "the Copenhagen interpretation takes stated probability at face value");
    }

    [Fact]
    public void PilotWave_InterpretProbability_CollapsesProbabilityToZeroOrOne_RegardlessOfRawSample()
    {
        // arrange — Pilot Wave is deterministic; rawSample is entirely irrelevant (epistemic randomness only)
        var interpretation = QuantumInterpretations.PilotWave;

        // act — vary rawSample across the full [0,1) range; result must be the same each time
        var highWithLowSample = interpretation.InterpretProbability(0.1, 0.8);
        var highWithHighSample = interpretation.InterpretProbability(0.9, 0.8);
        var lowWithLowSample = interpretation.InterpretProbability(0.1, 0.3);
        var lowWithHighSample = interpretation.InterpretProbability(0.9, 0.3);

        // assert — rawSample has no effect; only statedProbability matters
        highWithLowSample.Should().Be(1.0, "statedProbability >= 0.5 collapses to certainty, regardless of rawSample");
        highWithHighSample.Should().Be(1.0, "the pilot wave has already determined the outcome; the raw sample is irrelevant");
        lowWithLowSample.Should().Be(0.0, "statedProbability < 0.5 collapses to impossibility, regardless of rawSample");
        lowWithHighSample.Should().Be(0.0, "no amount of random sampling overrides the deterministic pilot wave");
    }

    [Fact]
    public void QBist_InterpretProbability_BoostsProbabilityByConfidence()
    {
        // arrange — QBism adds a confidence boost to stated probability
        var interpretation = QuantumInterpretations.QBist;

        // act
        var result = interpretation.InterpretProbability(0.5, 0.5);

        // assert — should be above 0.5 (boosted), clamped to [0,1]
        result.Should().BeGreaterThan(0.5, "QBism grants a modest confidence boost — believing you will succeed is a valid epistemic input");
        result.Should().BeLessThanOrEqualTo(1.0, "probability is still clamped to [0,1], even under the influence of personal conviction");
    }

    [Fact]
    public void Transactional_InterpretProbability_AmplifiesHighProbability()
    {
        // arrange — Transactional amplifies probabilities above 0.5 toward 1, and suppresses low ones toward 0
        var interpretation = QuantumInterpretations.Transactional;

        // act — p=0.8: p^(2/3) ≈ 0.862, amplified upward toward 1
        var high = interpretation.InterpretProbability(0.5, 0.8);

        // p=0.2: p^(3/2) ≈ 0.089, suppressed downward toward 0
        var low = interpretation.InterpretProbability(0.5, 0.2);

        // assert
        high.Should().BeGreaterThan(0.8, "the Transactional interpretation amplifies high probabilities — the handshake between offer and confirmation waves reinforces strong timelines");
        high.Should().BeLessThanOrEqualTo(1.0, "amplification is clamped at 1.0 even under retrocausal reinforcement");
        low.Should().BeLessThan(0.2, "Transactional decay suppresses weak timelines — the future does not confirm offers that were never serious");
    }

    // ──────────────────────────────────────────────────────────────────────────
    // ObserverDependentValue — interpretation-driven behaviour
    // ──────────────────────────────────────────────────────────────────────────

    [Fact]
    public void ObserverDependentValue_ManyWorlds_IgnoresRegistrationAndReturnsDefault()
    {
        // arrange — in Many-Worlds, observation does not affect outcome
        var value = new ObserverDependentValue<string>("universe default", interpretation: QuantumInterpretations.ManyWorlds)
            .For("Observe_RegisteredCaller_ReceivesPersonalisedValue", "personalised value");

        // act — caller is registered but interpretation overrides
        var result = value.Observe("Observe_RegisteredCaller_ReceivesPersonalisedValue");

        // assert
        result.Should().Be("universe default",
            "the Many-Worlds interpretation holds that observation does not affect outcome; all observers receive the same universal default");
    }

    [Fact]
    public void ObserverDependentValue_Copenhagen_HonoursRegistration()
    {
        // arrange — Copenhagen is the default; registration is respected
        var value = new ObserverDependentValue<string>("default", interpretation: QuantumInterpretations.Copenhagen)
            .For("specific-caller", "specific-value");

        // act
        var result = value.Observe("specific-caller");

        // assert
        result.Should().Be("specific-value", "the Copenhagen interpretation collapses to the observer-specific value");
    }

    // ──────────────────────────────────────────────────────────────────────────
    // CollapseOnReview — interpretation-driven behaviour
    // ──────────────────────────────────────────────────────────────────────────

    [Fact]
    public void CollapseOnReview_ManyWorlds_AlwaysReturnsProductionValue()
    {
        // arrange — Many-Worlds: observation never causes collapse
        var value = new CollapseOnReview<string>(
            productionValue: "production",
            reviewValue: "reviewed",
            interpretation: QuantumInterpretations.ManyWorlds);

        // act — this runs inside xUnit (review context detected), but interpretation overrides
        var result = value.Resolve();

        // assert
        result.Should().Be("production",
            "under Many-Worlds, the wavefunction never collapses — both production and review values coexist, and we return the production one");
    }

    [Fact]
    public void CollapseOnReview_Copenhagen_CollapsesUnderReviewContext()
    {
        // arrange — Copenhagen: observation collapses the wavefunction
        var value = new CollapseOnReview<string>(
            productionValue: "production",
            reviewValue: "reviewed",
            interpretation: QuantumInterpretations.Copenhagen);

        // act — running inside xUnit triggers review context detection
        var result = value.Resolve();

        // assert
        result.Should().Be("reviewed",
            "under Copenhagen, the test framework's presence on the call stack constitutes observation, causing collapse to the review value");
    }

    // ──────────────────────────────────────────────────────────────────────────
    // GetCollapseBehavior — each interpretation returns a non-null behavior
    // ──────────────────────────────────────────────────────────────────────────

    [Theory]
    [MemberData(nameof(AllInterpretations))]
    public void AllInterpretations_GetCollapseBehavior_ReturnsNonNullBehavior(IQuantumInterpretation interpretation)
    {
        // act
        var behavior = interpretation.GetCollapseBehavior<string>();

        // assert
        behavior.Should().NotBeNull("every interpretation must supply a collapse behavior — even if that behavior is 'do nothing useful'");
    }

    public static IEnumerable<object[]> AllInterpretations() =>
        QuantumInterpretations.All.Select(i => new object[] { i });
}
