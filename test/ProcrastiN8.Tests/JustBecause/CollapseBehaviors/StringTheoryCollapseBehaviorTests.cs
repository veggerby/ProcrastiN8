using ProcrastiN8.JustBecause;
using ProcrastiN8.JustBecause.CollapseBehaviors;

namespace ProcrastiN8.Tests.JustBecause.CollapseBehaviors;

public class StringTheoryCollapseBehaviorTests
{
    [Fact]
    public async Task CollapseAsync_Should_Resolve_When_11_Threads_Wait()
    {
        // Arrange
        var behavior = new StringTheoryCollapseBehavior<string>();
        var promise = Substitute.For<IQuantumPromise<string>>();
        promise.Value.Returns("ResolvedValue");

        var promises = new List<IQuantumPromise<string>> { promise };
        var tasks = Enumerable.Range(0, 11).Select(_ => Task.Run(async () =>
        {
            return await behavior.CollapseAsync(promises, CancellationToken.None);
        }));

        // Act
        var results = await Task.WhenAll(tasks);

        // Assert
        Assert.All(results, result => Assert.Equal("ResolvedValue", result));
    }

    [Fact]
    public async Task CollapseAsync_Should_Block_When_Less_Than_11_Threads_Wait()
    {
        // Arrange
        var behavior = new StringTheoryCollapseBehavior<string>();
        var promise = Substitute.For<IQuantumPromise<string>>();
        promise.Value.Returns("ResolvedValue");
        var promises = new List<IQuantumPromise<string>> { promise };
        var cts = new CancellationTokenSource(100); // Cancel after 100ms
        var tasks = Enumerable.Range(0, 10).Select(_ => Task.Run(async () =>
        {
            return await behavior.CollapseAsync(promises, cts.Token);
        }));

        // Act & Assert
        await Assert.ThrowsAsync<TaskCanceledException>(async () =>
        {
            await Task.WhenAll(tasks);
        });
    }
}