using ProcrastiN8.JustBecause;
using ProcrastiN8.JustBecause.CollapseBehaviors;

namespace ProcrastiN8.Tests.JustBecause.CollapseBehaviors;

public class SpookyActionCollapseBehaviorTests
{
    [Fact]
    public async Task CollapseAsync_Should_Collapse_All_Entangled_Promises()
    {
        // Arrange
        var promise1 = new QuantumPromise<string>(() => Task.FromResult("spooky1"), TimeSpan.FromSeconds(1));
        var promise2 = new QuantumPromise<string>(() => Task.FromResult("spooky2"), TimeSpan.FromSeconds(1));
        await Task.Delay(1100); // ensure not too early
        var entangled = new List<QuantumPromise<string>> { promise1, promise2 };
        var behavior = new SpookyActionCollapseBehavior<string>();

        // Act
        var result = await behavior.CollapseAsync(entangled, CancellationToken.None);

        // Assert
        result.Should().Be("spooky1", "the first promise should be observed, all collapse together");
        // This test does not verify registry logic, only the behavior's contract.
        // Spooky action at a distance, now with unit tests and serious intent.
    }
}