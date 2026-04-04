using ProcrastiN8.JustBecause;

namespace ProcrastiN8.Tests.JustBecause;

public class ObserverDependentValueTests
{
    [Fact]
    public void Observe_UnregisteredCaller_ReturnsDefault()
    {
        // arrange — a value with a known default for unregistered observers
        var value = new ObserverDependentValue<string>("default answer");

        // act — call from an unregistered context
        var result = value.Observe();

        // assert
        result.Should().Be("default answer", "unregistered observers receive the universal truth, which is the default");
    }

    [Fact]
    public void Observe_RegisteredCaller_ReceivesPersonalisedValue()
    {
        // arrange — register a specific value for this very method
        var value = new ObserverDependentValue<int>(0)
            .For("Observe_RegisteredCaller_ReceivesPersonalisedValue", 42);

        // act — calling from this exact method name triggers the registered response
        var result = value.Observe();

        // assert
        result.Should().Be(42, "the observer with a specific registration receives their personalised truth");
    }

    [Fact]
    public void For_RegistersMultiple_Observers()
    {
        // arrange
        var value = new ObserverDependentValue<string>("nobody")
            .For("Alice", "Alice's truth")
            .For("Bob", "Bob's truth");

        // assert
        value.RegisteredObserverCount.Should().Be(2, "two observers were registered, each with their own reality");
        value.RegisteredObservers.Should().Contain("Alice").And.Contain("Bob");
    }

    [Fact]
    public void Observe_DifferentCallers_ReceiveDifferentValues()
    {
        // arrange — two methods will call observe; both are registered with different values
        var value = new ObserverDependentValue<string>("default")
            .For("Method_A", "value for A")
            .For("Method_B", "value for B");

        // act — simulate each caller using direct key lookup (since CallerMemberName is compile-time)
        var resultA = value.Observe("Method_A");
        var resultB = value.Observe("Method_B");
        var resultDefault = value.Observe("Method_C");

        // assert — each observer collapses the value differently
        resultA.Should().Be("value for A");
        resultB.Should().Be("value for B");
        resultDefault.Should().Be("default", "unregistered observers are given the universal default without complaint");
    }
}
