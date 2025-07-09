using System;
using System.Threading;
using System.Threading.Tasks;

using ProcrastiN8.JustBecause;

using Xunit;

namespace ProcrastiN8.Tests.JustBecause
{
    public class QuantumPromiseTests
    {
        [Fact]
        public async Task ObserveAsync_ReturnsValue_WhenObservedInWindow()
        {
            // Arrange
            var promise = new QuantumPromise<int>(() => Task.FromResult(123), TimeSpan.FromSeconds(2));
            await Task.Delay(1100); // ensure not too early

            // Act
            var result = await promise.ObserveAsync();

            // Assert
            result.Should().Be(123);
            promise.ToString().Should().Contain("resolved");
        }

        [Fact]
        public async Task ObserveAsync_ThrowsTooEarly()
        {
            // Arrange
            var promise = new QuantumPromise<int>(() => Task.FromResult(1), TimeSpan.FromSeconds(2));

            // Act
            Func<Task> act = async () => await promise.ObserveAsync();

            // Assert
            await act.Should().ThrowAsync<CollapseTooEarlyException>();
        }

        [Fact]
        public async Task ObserveAsync_ThrowsTooLate()
        {
            // Arrange
            var promise = new QuantumPromise<int>(() => Task.FromResult(1), TimeSpan.FromMilliseconds(10));
            await Task.Delay(50); // ensure too late

            // Act
            Func<Task> act = async () => await promise.ObserveAsync();

            // Assert
            await act.Should().ThrowAsync<CollapseTooLateException>();
        }

        [Fact]
        public async Task ObserveAsync_ThrowsIfInitializerThrows()
        {
            // Arrange
            var promise = new QuantumPromise<int>(() => throw new InvalidOperationException("fail"), TimeSpan.FromSeconds(2));
            await Task.Delay(1100); // ensure not too early
            // Act
            Func<Task> act = async () => await promise.ObserveAsync();
            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>();
        }

        [Fact]
        public async Task ObserveAsync_ThrowsVoidCollapseSometimes()
        {
            // Arrange
            var triggered = false;
            for (int i = 0; i < 20; i++)
            {
                var promise = new QuantumPromise<int>(() => Task.FromResult(1), TimeSpan.FromSeconds(2));
                await Task.Delay(1100);

                // Act
                try { await promise.ObserveAsync(); } catch (CollapseToVoidException) { triggered = true; break; } catch { }
            }

            // Assert
            triggered.Should().BeTrue();
        }

        [Fact]
        public async Task ObserveAsync_ThrowsOnSecondObserveIfFailed()
        {
            // Arrange
            var promise = new QuantumPromise<int>(() => throw new InvalidOperationException("fail"), TimeSpan.FromSeconds(2));
            await Task.Delay(1100);

            // Act
            Func<Task> act = async () => await promise.ObserveAsync();
            await act.Should().ThrowAsync<InvalidOperationException>();
            Action act2 = () => promise.ObserveAsync().GetAwaiter().GetResult();

            // Assert
            act2.Should().Throw<InvalidOperationException>();
        }
    }
}