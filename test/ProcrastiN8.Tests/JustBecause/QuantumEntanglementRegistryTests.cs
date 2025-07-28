using ProcrastiN8.JustBecause;
using ProcrastiN8.JustBecause.CollapseBehaviors;

namespace ProcrastiN8.Tests.JustBecause;

public class QuantumEntanglementRegistryTests
{
    [Fact]
    public void ToString_ReportsCount()
    {
        // Arrange
        var reg = new QuantumEntanglementRegistry<int>();
        // Act & Assert
        reg.ToString().Should().Contain("0", "the registry should begin in a state of quantum loneliness");
        reg.Entangle(MakePromise());
        reg.ToString().Should().Contain("1", "entanglement should be reflected in the registry's existential count");
    }

    [Fact]
    public void Entangle_AddsPromise()
    {
        // Arrange
        var reg = new QuantumEntanglementRegistry<int>();
        var promise = MakePromise(42);
        // Act
        reg.Entangle(promise);
        // Assert
        reg.ToString().Should().Contain("1", "entangling a promise should increment the registry's quantum population");
    }

    [Fact]
    public async Task CollapseOneAsync_DelegatesToBehavior()
    {
        // Arrange
        var behavior = Substitute.For<ICollapseBehavior<int>>();
        var reg = new QuantumEntanglementRegistry<int>(behavior);
        var promise = MakePromise(99);
        reg.Entangle(promise);
        behavior.CollapseAsync(Arg.Any<IEnumerable<QuantumPromise<int>>>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<int>(99));
        // Act
        var result = await reg.CollapseOneAsync();
        // Assert
        result.Should().Be(99, "the registry should delegate collapse to its behavior, regardless of quantum ethics");
        await behavior.Received(1).CollapseAsync(Arg.Any<IEnumerable<QuantumPromise<int>>>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ObserveAsync_CollapsesViaRegistryAndBehavior()
    {
        // Arrange
        var behavior = Substitute.For<ICollapseBehavior<int>>();
        var reg = new QuantumEntanglementRegistry<int>(behavior);
        var promise = MakePromise(42);
        reg.Entangle(promise);
        behavior.CollapseAsync(Arg.Any<IEnumerable<IQuantumPromise<int>>>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<int>(42));
        // Act
        var result = await reg.ObserveAsync(promise);
        // Assert
        result.Should().Be(42, "the registry should coordinate collapse and return the observed value");
        await behavior.Received(1).CollapseAsync(Arg.Any<IEnumerable<IQuantumPromise<int>>>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public void Entangle_ThrowsOnNull()
    {
        // Arrange
        var reg = new QuantumEntanglementRegistry<int>();
        // Act
        Action act = () => reg.Entangle(null!);
        // Assert
        act.Should().Throw<ArgumentNullException>("entangling null should be forbidden, even in quantum C#");
    }

    [Fact]
    public void ObserveAsync_ThrowsIfPromiseNotEntangled()
    {
        // Arrange
        var reg = new QuantumEntanglementRegistry<int>();
        var promise = MakePromise(7);
        // Act
        Func<Task> act = async () => await reg.ObserveAsync(promise);
        // Assert
        act.Should().ThrowAsync<InvalidOperationException>("observing a non-entangled promise should be forbidden by quantum law");
    }

    // Helper for creating promises
    private QuantumPromise<int> MakePromise(int value = 0)
    {
        // Use a 2 second window to avoid early/late collapse in tests
        return new QuantumPromise<int>(() => Task.FromResult(value), TimeSpan.FromSeconds(2));
    }
}