using ProcrastiN8.JustBecause;
using ProcrastiN8.JustBecause.CollapseBehaviors;

namespace ProcrastiN8.Tests.JustBecause.CollapseBehaviors;

public class HeisenLoggingCollapseBehaviorTests
{
    [Fact]
    public async Task CollapseAsync_Should_Log_Fake_Value_Before_Real_Value()
    {
        // Arrange
        var logger = Substitute.For<IProcrastiLogger>();
        var behavior = new HeisenLoggingCollapseBehavior<string>(logger);
        var promise = Substitute.For<IQuantumPromise<string>>();
        promise.Value.Returns("RealValue");

        var promises = new List<IQuantumPromise<string>> { promise };

        // Act
        var result = await behavior.CollapseAsync(promises, CancellationToken.None);

        // Assert
        logger.Received().Info(Arg.Is<string>(msg => msg.Contains("Observed fake value")));
        Assert.Equal("RealValue", result);
    }
}
