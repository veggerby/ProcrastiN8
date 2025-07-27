using ProcrastiN8.JustBecause;
using ProcrastiN8.JustBecause.CollapseBehaviors;
using ProcrastiN8.Tests.Common;

namespace ProcrastiN8.Tests.JustBecause.CollapseBehaviors;

public class SpookyActionCollapseBehaviorTests
{
    [Fact]
    public async Task CollapseAsync_Should_Collapse_All_Entangled_Promises()
    {
        // Arrange
        var promise1 = new PredictableQuantumPromise<string>("spooky1");
        var promise2 = new PredictableQuantumPromise<string>("spooky2");
        var entangled = new List<IQuantumPromise<string>> { promise1, promise2 };
        var behavior = new SpookyActionCollapseBehavior<string>();

        // Act
        var result = await behavior.CollapseAsync(entangled, CancellationToken.None);

        // Assert
        result.Should().Be("spooky1", "the first promise should be observed, all collapse together");
        // This test does not verify registry logic, only the behavior's contract.
        // Spooky action at a distance, now with unit tests and serious intent.
    }
}