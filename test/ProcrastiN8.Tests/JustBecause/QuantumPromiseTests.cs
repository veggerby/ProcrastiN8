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
        await Task.Delay(1100); // ensure not too early

        // Act
        int? result = null;
        Exception? exception = null;
        try { result = await promise.ObserveAsync(); } catch (Exception ex) { exception = ex; }

        // Assert
        if (exception is not null)
        {
            exception.Should().BeOfType<CollapseToVoidException>("quantum promises may collapse to void"); // This branch should never be hit with PredictableQuantumPromise
        }
        else
        {
            result.Should().Be(123, "the quantum promise should resolve to the expected value");
            promise.ToString().Should().Contain("PredictableQuantumPromise");
        }
    }

    [Fact]
    public async Task ObserveAsync_ThrowsTooEarly_OrVoidCollapse()
    {
        // Arrange
        var promise = new QuantumPromise<int>(() => Task.FromResult(1), TimeSpan.FromSeconds(2));

        // Act
        Func<Task> act = async () => await promise.ObserveAsync();

        // Assert
        await act.Should().ThrowAsync<Exception>()
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

    [Fact(Skip = "This test is flaky and needs to be stabilized")]
    public async Task ObserveAsync_ThrowsVoidCollapseSometimes()
    {
        // Arrange
        var triggered = false;
        for (int i = 0; i < 30; i++)
        {
            var promise = new QuantumPromise<int>(() => Task.FromResult(1), TimeSpan.FromSeconds(2));
            await Task.Delay(1100);
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
}