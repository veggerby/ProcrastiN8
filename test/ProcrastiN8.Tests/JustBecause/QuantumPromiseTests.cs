using ProcrastiN8.JustBecause;
using ProcrastiN8.Tests.Common;

namespace ProcrastiN8.Tests.JustBecause;

public class QuantumPromiseTests
{
    [Fact]
    public async Task ObserveAsync_ReturnsValue_WhenObservedInWindow()
    {
        // Arrange
        var promise = new PredictableQuantumPromise<int>(123);
        // Act
        var result = await promise.ObserveAsync();
        // Assert
        result.Should().Be(123, "the quantum promise should resolve to the expected value");
        promise.ToString().Should().Contain("PredictableQuantumPromise");
    }

    [Fact]
    public async Task ObserveAsync_ThrowsTooEarly_OrVoidCollapse()
    {
        // Arrange
        var promise = new QuantumPromise<int>(() => Task.FromResult(1), TimeSpan.FromSeconds(2));
        // Act
        Func<Task> act = async () => await promise.ObserveAsync();
        // Assert
        await act.Should().ThrowAsync<CollapseException>()
            .Where(e => e is CollapseTooEarlyException || e is CollapseToVoidException);
    }

    [Fact]
    public async Task ObserveAsync_ThrowsTooLate_OrVoidCollapse()
    {
        // Arrange
        var promise = new QuantumPromise<int>(() => Task.FromResult(1), TimeSpan.FromMilliseconds(10));
        await Task.Delay(1100); // ensure too late
        // Act
        Func<Task> act = async () => await promise.ObserveAsync();
        // Assert
        await act.Should().ThrowAsync<CollapseException>()
            .Where(e => e is CollapseTooLateException || e is CollapseToVoidException);
    }

    [Fact]
    public async Task ObserveAsync_ThrowsIfInitializerThrows()
    {
        // Arrange
        var promise = new AlwaysThrowsQuantumPromise<int>(new InvalidOperationException("fail"));
        // Act
        Func<Task> act = async () => await promise.ObserveAsync();
        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>("the predictable promise should always throw");
    }

    [Fact]
    public async Task ObserveDecisionAsync_TriggersEntangledCallback()
    {
        // Arrange
        string? observed = null;
        void Handler(string s) => observed = s;
        QuantumUndecider.OnEntangledDecision += Handler;

        try
        {
            var mockRandomProvider = Substitute.For<IRandomProvider>();
            QuantumUndecider.SetRandomProvider(mockRandomProvider);

            // Ensure callback is triggered by aligning with the "It depends." decision state
            mockRandomProvider.NextDouble().Returns(0.2); // Ensure partial decision condition is met
            mockRandomProvider.Next(Arg.Any<int>(), Arg.Any<int>()).Returns(2); // Index for "It depends."

            // Act
            await QuantumUndecider.ObserveDecisionAsync(() => Task.FromResult(true));

            // Assert
            observed.Should().Be("It depends.", "the entangled callback should always be triggered with the correct state");
        }
        finally
        {
            QuantumUndecider.OnEntangledDecision -= Handler;
        }
    }

    [Fact]
    public async Task ObserveAsync_ThrowsVoidCollapseSometimes()
    {
        // Arrange
        var triggered = false;
        var mockRandomProvider = Substitute.For<IRandomProvider>();
        mockRandomProvider.NextDouble().Returns(0.05); // Ensure CollapseToVoidException is triggered

        for (int i = 0; i < 3; i++) // Reduce iterations
        {
            var promise = new QuantumPromise<int>(() => Task.FromResult(1), TimeSpan.FromSeconds(2), randomProvider: mockRandomProvider);
            await Task.Delay(500); // Reduce delay
            try { await promise.ObserveAsync(); }
            catch (CollapseToVoidException)
            {
                triggered = true;
                break;
            }
            catch { /* ignore other exceptions for this test */ }
        }

        // Assert
        triggered.Should().BeTrue("eventually, the quantum promise must collapse to void (by design)");
    }

    [Fact]
    public async Task ObserveAsync_ThrowsOnSecondObserveIfFailed_OrVoidCollapse()
    {
        // Arrange
        var promise = new QuantumPromise<int>(() => throw new InvalidOperationException("fail"), TimeSpan.FromSeconds(2));
        await Task.Delay(1100);

        // Act
        Exception? first = null;
        try { await promise.ObserveAsync(); } catch (Exception ex) { first = ex; }
        Action act2 = () => promise.ObserveAsync().GetAwaiter().GetResult();

        // Assert
        if (first is CollapseToVoidException)
        {
            act2.Should().Throw<CollapseToVoidException>("the void is persistent");
        }
        else
        {
            act2.Should().Throw<InvalidOperationException>("the original failure should persist");
        }
    }

    [Fact]
    public void ToString_ReportsStatus()
    {
        // Arrange
        var promise = new QuantumPromise<int>(() => Task.FromResult(7), TimeSpan.FromSeconds(2));

        // Act
        var str = promise.ToString();

        // Assert
        str.Should().Contain("QuantumPromise", "the string representation should be suitably dramatic");
    }

    [Fact]
    public async Task ObserveAsync_DelegatesToEntanglementRegistry()
    {
        // arrange
        var registry = Substitute.For<IQuantumEntanglementRegistry<int>>();
        var promise = new QuantumPromise<int>(() => Task.FromResult(42), TimeSpan.FromSeconds(2));
        registry.ObserveAsync(promise, Arg.Any<CancellationToken>()).Returns(Task.FromResult(99));
        // Use internal method to set registry (simulate entanglement)
        typeof(QuantumPromise<int>).GetMethod("SetEntanglementRegistry", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!
            .Invoke(promise, new object[] { registry });

        // act
        var result = await promise.ObserveAsync();

        // assert
        result.Should().Be(99, "entangled promises must defer to their registry for collapse");
        await registry.Received(1).ObserveAsync(promise, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CollapseToValueAsync_ForciblyResolvesPromise()
    {
        // arrange
        var promise = new QuantumPromise<string>(() => Task.FromResult("never used"), TimeSpan.FromSeconds(2));
        var collapsible = (ICopenhagenCollapsible<string>)promise;

        // act
        await collapsible.CollapseToValueAsync("collapsed");
        var result = await promise.ObserveAsync();

        // assert
        result.Should().Be("collapsed", "Copenhagen collapse forcibly resolves the promise");
        promise.ToString().Should().Contain("resolved");
    }
}