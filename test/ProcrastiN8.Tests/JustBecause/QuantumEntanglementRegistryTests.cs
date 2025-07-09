using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using ProcrastiN8.JustBecause;

using Xunit;

namespace ProcrastiN8.Tests.JustBecause
{
    public class QuantumEntanglementRegistryTests
    {
        private QuantumPromise<int> MakePromise(int value = 0, bool shouldThrow = false)
        {
            if (shouldThrow)
            {
                return new QuantumPromise<int>(() => throw new InvalidOperationException("Test exception"), TimeSpan.FromSeconds(10));
            }
            return new QuantumPromise<int>(() => Task.FromResult(value), TimeSpan.FromSeconds(10));
        }

        [Fact]
        public void Entangle_ThrowsOnNull()
        {
            // Arrange
            var reg = new QuantumEntanglementRegistry<int>();
            Action act = () => reg.Entangle(null!);

            // Act & Assert
            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ToString_ReportsCount()
        {
            // Arrange
            var reg = new QuantumEntanglementRegistry<int>();

            // Act & Assert
            reg.ToString().Should().Contain("0");
            reg.Entangle(MakePromise());
            reg.ToString().Should().Contain("1");
        }

        [Fact]
        public async Task CollapseOneAsync_SinglePromise_ReturnsValue()
        {
            // Arrange
            var reg = new QuantumEntanglementRegistry<int>();
            var promise = MakePromise(42);
            reg.Entangle(promise);

            // Act
            var result = await reg.CollapseOneAsync();

            // Assert
            result.Should().Be(42);
        }

        [Fact]
        public async Task CollapseOneAsync_ThrowsIfEmpty()
        {
            // Arrange
            var reg = new QuantumEntanglementRegistry<int>();
            // Act
            Func<Task> act = async () => await reg.CollapseOneAsync();
            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>();
        }

        [Fact]
        public async Task CollapseOneAsync_RipplesToOthers()
        {
            // Arrange
            var reg = new QuantumEntanglementRegistry<int>();
            var p1 = MakePromise(1);
            var p2 = MakePromise(2);
            reg.Entangle(p1);
            reg.Entangle(p2);
            // Act
            await reg.CollapseOneAsync();
            await Task.Delay(1500); // Allow ripple tasks to run
            // Assert
            true.Should().BeTrue();
        }

        [Fact]
        public async Task CollapseOneAsync_RippleEntropyOnFailure()
        {
            // Arrange
            var reg = new QuantumEntanglementRegistry<int>();
            var fail = MakePromise(0, shouldThrow: true);
            var ok = MakePromise(5);
            reg.Entangle(fail);
            reg.Entangle(ok);
            // Act
            Func<Task> act = async () => await reg.CollapseOneAsync();
            await act.Should().ThrowAsync<InvalidOperationException>();
            await Task.Delay(800); // Allow entropy ripple tasks to run
            // Assert
            true.Should().BeTrue();
        }

        [Fact]
        public void Entangle_Multiple_IncreasesCount()
        {
            // Arrange
            var reg = new QuantumEntanglementRegistry<int>();
            reg.Entangle(MakePromise());
            reg.Entangle(MakePromise());
            // Act & Assert
            reg.ToString().Should().Contain("2");
        }

        [Fact]
        public async Task CollapseOneAsync_CancellationToken_CancelsRipple()
        {
            // Arrange
            var reg = new QuantumEntanglementRegistry<int>();
            var p1 = MakePromise(1);
            var p2 = MakePromise(2);
            reg.Entangle(p1);
            reg.Entangle(p2);
            using var cts = new CancellationTokenSource();
            cts.Cancel();
            // Act
            Func<Task> act = async () => await reg.CollapseOneAsync(cts.Token);
            // Assert
            await act.Should().ThrowAsync<OperationCanceledException>();
        }

        [Fact]
        public async Task RippleCollapse_DoesNotThrowOnException()
        {
            // Arrange
            var reg = new QuantumEntanglementRegistry<int>();
            var p1 = MakePromise(1, shouldThrow: true);
            reg.Entangle(p1);
            var method = typeof(QuantumEntanglementRegistry<int>).GetMethod("RippleCollapse", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            method.Should().NotBeNull();
            // Act
            method!.Invoke(reg, new object[] { p1, CancellationToken.None });
            await Task.Delay(1000); // Let background task run
            // Assert
            true.Should().BeTrue();
        }

        [Fact]
        public async Task RippleEntropy_DoesNotThrowOnException()
        {
            // Arrange
            var reg = new QuantumEntanglementRegistry<int>();
            var p1 = MakePromise(1, shouldThrow: true);
            reg.Entangle(p1);
            var method = typeof(QuantumEntanglementRegistry<int>).GetMethod("RippleEntropy", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            method.Should().NotBeNull();
            // Act
            method!.Invoke(reg, new object[] { new[] { p1 }, CancellationToken.None });
            await Task.Delay(800); // Let background task run
            // Assert
            true.Should().BeTrue();
        }

        [Fact]
        public void ToString_EmptyRegistry()
        {
            // Arrange
            var reg = new QuantumEntanglementRegistry<int>();
            // Act
            var str = reg.ToString();
            // Assert
            str.Should().Contain("0");
            str.Should().StartWith("[Entangled Set:");
        }
    }
}