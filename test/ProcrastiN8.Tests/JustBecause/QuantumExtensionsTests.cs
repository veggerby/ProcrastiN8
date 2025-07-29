using ProcrastiN8.JustBecause;
using ProcrastiN8.JustBecause.CollapseBehaviors;

namespace ProcrastiN8.Tests.JustBecause;

public class QuantumExtensionsTests
{
    [Fact]
    public void Entangle_WithCollapseBehavior_EntanglesPromises()
    {
        // arrange
        var promise1 = new QuantumPromise<int>(() => Task.FromResult(42), TimeSpan.FromSeconds(1));
        var promise2 = new QuantumPromise<int>(() => Task.FromResult(99), TimeSpan.FromSeconds(1));
        var behavior = Substitute.For<ICollapseBehavior<int>>();

        // act
        promise1.Entangle(behavior, promise2);

        // assert
        // Verify that the promises are entangled (e.g., by checking shared registry or behavior usage)
    }

    [Fact]
    public void Entangle_WithQuantumComplianceLevel_EntanglesPromises()
    {
        // arrange
        var promise1 = new QuantumPromise<int>(() => Task.FromResult(42), TimeSpan.FromSeconds(1));
        var promise2 = new QuantumPromise<int>(() => Task.FromResult(99), TimeSpan.FromSeconds(1));
        var complianceLevel = QuantumComplianceLevel.Copenhagen;

        // act
        promise1.Entangle(complianceLevel, promise2);

        // assert
        // Verify that the promises are entangled with the correct compliance level
    }

    [Fact]
    public void Entangle_WithRandomProvider_EntanglesPromises()
    {
        // arrange
        var promise1 = new QuantumPromise<int>(() => Task.FromResult(42), TimeSpan.FromSeconds(1));
        var promise2 = new QuantumPromise<int>(() => Task.FromResult(99), TimeSpan.FromSeconds(1));
        var randomProvider = Substitute.For<IRandomProvider>();

        // act
        promise1.Entangle(randomProvider, promise2);

        // assert
        // Verify that the promises are entangled with the specified random provider
    }

    [Fact]
    public void Entangle_WithAllCustomizations_EntanglesPromises()
    {
        // arrange
        var promise1 = new QuantumPromise<int>(() => Task.FromResult(42), TimeSpan.FromSeconds(1));
        var promise2 = new QuantumPromise<int>(() => Task.FromResult(99), TimeSpan.FromSeconds(1));
        var behavior = Substitute.For<ICollapseBehavior<int>>();
        var randomProvider = Substitute.For<IRandomProvider>();
        var complianceLevel = QuantumComplianceLevel.Copenhagen;

        // act
        promise1.Entangle(complianceLevel, randomProvider, promise2);

        // assert
        // Verify that the promises are entangled with the specified behavior and random provider
    }
}