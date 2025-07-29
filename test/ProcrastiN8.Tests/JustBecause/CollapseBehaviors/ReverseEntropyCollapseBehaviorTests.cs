using ProcrastiN8.LazyTasks;
using ProcrastiN8.JustBecause;
using ProcrastiN8.JustBecause.CollapseBehaviors;

namespace ProcrastiN8.Tests.JustBecause.CollapseBehaviors;

public class ReverseEntropyCollapseBehaviorTests
{
    [Fact]
    public async Task CollapseAsync_Should_Decay_Over_Time()
    {
        // Arrange
        var creationTime = DateTimeOffset.UtcNow.AddSeconds(-10);
        var mockTimeProvider = Substitute.For<ITimeProvider>();
        mockTimeProvider.GetUtcNow().Returns(creationTime.AddSeconds(10)); // Simulate 10 seconds elapsed

        var mockRandomProvider = Substitute.For<IRandomProvider>();
        mockRandomProvider.NextDouble().Returns(0.9); // Ensure decay probability is high

        var behavior = new ReverseEntropyCollapseBehavior<string>(mockTimeProvider, mockRandomProvider);
        var promise = Substitute.For<IQuantumPromise<string>>();
        promise.CreationTime.Returns(creationTime.UtcDateTime);
        promise.Value.Returns("TestValue");

        var promises = new List<IQuantumPromise<string>> { promise };

        // Act & Assert
        await Assert.ThrowsAsync<CollapseExpiredException>(async () =>
        {
            await behavior.CollapseAsync(promises, CancellationToken.None);
        });
    }
}
