using ProcrastiN8.JustBecause;
using ProcrastiN8.LazyTasks;

namespace ProcrastiN8.Tests.JustBecause;

public class TimelineBranchTests
{
    [Fact]
    public void Constructor_Initializes_Properties_Correctly()
    {
        // arrange
        var branchId = "alternate-reality-1";
        var divergencePoint = new DateTimeOffset(2024, 1, 1, 12, 0, 0, TimeSpan.Zero);

        // act
        var branch = new TimelineBranch(branchId, divergencePoint);

        // assert
        branch.BranchId.Should().Be(branchId, "branch ID is set correctly");
        branch.DivergencePoint.Should().Be(divergencePoint, "divergence point is recorded");
        branch.ObservationCount.Should().Be(0, "no observations yet");
    }

    [Fact]
    public void Constructor_Throws_For_Null_BranchId()
    {
        // arrange
        var divergencePoint = new DateTimeOffset(2024, 1, 1, 12, 0, 0, TimeSpan.Zero);

        // act
        Action act = () => new TimelineBranch(null!, divergencePoint);

        // assert
        act.Should().Throw<ArgumentNullException>("branch ID cannot be null");
    }

    [Fact]
    public void GetDivergenceIndex_Increases_With_Time()
    {
        // arrange
        var divergencePoint = new DateTimeOffset(2024, 1, 1, 12, 0, 0, TimeSpan.Zero);
        var timeProvider = Substitute.For<ITimeProvider>();
        timeProvider.GetUtcNow().Returns(
            divergencePoint.AddHours(1),
            divergencePoint.AddHours(5));
        
        var branch = new TimelineBranch("test", divergencePoint, timeProvider);

        // act
        var index1 = branch.GetDivergenceIndex();
        var index2 = branch.GetDivergenceIndex();

        // assert
        index2.Should().BeGreaterThan(index1, "divergence increases over time");
    }

    [Fact]
    public void GetDivergenceIndex_Increases_With_Observations()
    {
        // arrange
        var divergencePoint = new DateTimeOffset(2024, 1, 1, 12, 0, 0, TimeSpan.Zero);
        var timeProvider = Substitute.For<ITimeProvider>();
        timeProvider.GetUtcNow().Returns(divergencePoint.AddHours(1));
        
        var branch = new TimelineBranch("test", divergencePoint, timeProvider);

        // act
        var indexBefore = branch.GetDivergenceIndex();
        branch.RecordObservation();
        branch.RecordObservation();
        var indexAfter = branch.GetDivergenceIndex();

        // assert
        indexAfter.Should().BeGreaterThan(indexBefore, "observations amplify divergence");
    }

    [Fact]
    public void SetBranchState_And_GetBranchState_Work_Correctly()
    {
        // arrange
        var branch = new TimelineBranch("test", DateTimeOffset.UtcNow);
        var key = "test-key";
        var value = "test-value";

        // act
        branch.SetBranchState(key, value);
        var retrieved = branch.GetBranchState<string>(key);

        // assert
        retrieved.Should().Be(value, "state is stored and retrieved correctly");
    }

    [Fact]
    public void GetBranchState_Returns_Default_For_Missing_Key()
    {
        // arrange
        var branch = new TimelineBranch("test", DateTimeOffset.UtcNow);

        // act
        var result = branch.GetBranchState<string>("nonexistent");

        // assert
        result.Should().BeNull("missing keys return default");
    }

    [Fact]
    public void RecordObservation_Increments_Count()
    {
        // arrange
        var branch = new TimelineBranch("test", DateTimeOffset.UtcNow);

        // act
        branch.RecordObservation();
        branch.RecordObservation();
        branch.RecordObservation();

        // assert
        branch.ObservationCount.Should().Be(3, "three observations recorded");
    }

    [Fact]
    public void IsParadoxical_Returns_False_For_Low_Divergence()
    {
        // arrange
        var divergencePoint = new DateTimeOffset(2024, 1, 1, 12, 0, 0, TimeSpan.Zero);
        var timeProvider = Substitute.For<ITimeProvider>();
        timeProvider.GetUtcNow().Returns(divergencePoint.AddHours(1));
        
        var branch = new TimelineBranch("test", divergencePoint, timeProvider);

        // act
        var isParadoxical = branch.IsParadoxical(paradoxThreshold: 1000.0);

        // assert
        isParadoxical.Should().BeFalse("low divergence is not paradoxical");
    }

    [Fact]
    public void IsParadoxical_Returns_True_For_High_Divergence()
    {
        // arrange
        var divergencePoint = new DateTimeOffset(2024, 1, 1, 12, 0, 0, TimeSpan.Zero);
        var timeProvider = Substitute.For<ITimeProvider>();
        timeProvider.GetUtcNow().Returns(divergencePoint.AddDays(100));
        
        var branch = new TimelineBranch("test", divergencePoint, timeProvider);

        // act
        var isParadoxical = branch.IsParadoxical(paradoxThreshold: 10.0);

        // assert
        isParadoxical.Should().BeTrue("high divergence triggers paradox");
    }
}

public class TimelineBranchExtensionsTests
{
    [Fact]
    public void GetOrCreateBranch_Creates_New_Branch()
    {
        // arrange
        var branchId = $"test-branch-{Guid.NewGuid()}";

        // act
        var branch = TimelineBranchExtensions.GetOrCreateBranch(branchId);

        // assert
        branch.Should().NotBeNull("branch is created");
        branch.BranchId.Should().Be(branchId, "branch has correct ID");
    }

    [Fact]
    public void GetOrCreateBranch_Returns_Existing_Branch()
    {
        // arrange
        var branchId = $"test-branch-{Guid.NewGuid()}";
        var branch1 = TimelineBranchExtensions.GetOrCreateBranch(branchId);

        // act
        var branch2 = TimelineBranchExtensions.GetOrCreateBranch(branchId);

        // assert
        branch2.Should().BeSameAs(branch1, "same branch instance is returned");
    }

    [Fact]
    public async Task ObserveInBranchAsync_Records_Observation()
    {
        // arrange
        var branchId = $"test-branch-{Guid.NewGuid()}";
        var promise = new Tests.Common.PredictableQuantumPromise<int>(42);

        // act
        var result = await promise.ObserveInBranchAsync(branchId);

        // assert
        result.Should().Be(42, "promise collapses normally in alternate timeline");
        var branch = TimelineBranchExtensions.GetOrCreateBranch(branchId);
        branch.ObservationCount.Should().BeGreaterThan(0, "observation is recorded");
    }

    [Fact]
    public async Task IsObservedInBranch_Returns_True_After_Observation()
    {
        // arrange
        var branchId = $"test-branch-{Guid.NewGuid()}";
        var promise = new Tests.Common.PredictableQuantumPromise<string>("done");

        // act
        await promise.ObserveInBranchAsync(branchId);
        var isObserved = promise.IsObservedInBranch(branchId);

        // assert
        isObserved.Should().BeTrue("promise was observed in this branch");
    }

    [Fact]
    public void IsObservedInBranch_Returns_False_Before_Observation()
    {
        // arrange
        var branchId = $"test-branch-{Guid.NewGuid()}";
        var promise = new Tests.Common.PredictableQuantumPromise<string>("not done");

        // act
        var isObserved = promise.IsObservedInBranch(branchId);

        // assert
        isObserved.Should().BeFalse("promise has not been observed in this branch");
    }

    [Fact]
    public async Task Multiple_Promises_Can_Be_Observed_In_Same_Branch()
    {
        // arrange
        var branchId = $"test-branch-{Guid.NewGuid()}";
        var promise1 = new Tests.Common.PredictableQuantumPromise<int>(1);
        var promise2 = new Tests.Common.PredictableQuantumPromise<int>(2);

        // act
        var result1 = await promise1.ObserveInBranchAsync(branchId);
        var result2 = await promise2.ObserveInBranchAsync(branchId);

        // assert
        result1.Should().Be(1, "first promise resolves");
        result2.Should().Be(2, "second promise resolves");
        promise1.IsObservedInBranch(branchId).Should().BeTrue("first is observed");
        promise2.IsObservedInBranch(branchId).Should().BeTrue("second is observed");
    }
}
