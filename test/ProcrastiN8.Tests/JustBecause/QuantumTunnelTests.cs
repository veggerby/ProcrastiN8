using ProcrastiN8.JustBecause;

namespace ProcrastiN8.Tests.JustBecause;

public class QuantumTunnelTests
{
    [Fact]
    public async Task TunnelAsync_SuccessfulOperation_ReturnsResult()
    {
        // arrange — an operation that succeeds needs no tunnelling at all
        // act
        var result = await QuantumTunnel.TunnelAsync(() => Task.FromResult(42));

        // assert
        result.Should().Be(42, "a successful operation exits its timeline without quantum intervention");
    }

    [Fact]
    public async Task TunnelAsync_ThrowingOperation_ReturnsFallback_WhenTunnelingProbabilityIsOne()
    {
        // arrange — 100% tunnelling probability means the exception never wins
        var randomProvider = Substitute.For<IRandomProvider>();
        randomProvider.GetDouble().Returns(0.0); // always < 1.0, so always tunnels

        // act
        var result = await QuantumTunnel.TunnelAsync(
            () => throw new InvalidOperationException("classical physics"),
            fallback: -1,
            tunnelingProbability: 1.0,
            randomProvider: randomProvider);

        // assert
        result.Should().Be(-1, "the exception barrier was tunnelled; the fallback represents the other side");
    }

    [Fact]
    public async Task TunnelAsync_ThrowingOperation_Rethrows_WhenTunnelingFails()
    {
        // arrange — 0% tunnelling probability means the exception retains authority
        var randomProvider = Substitute.For<IRandomProvider>();
        randomProvider.GetDouble().Returns(1.0); // always >= 0.0, so tunnelling fails when probability < 1.0

        // act
        Func<Task> act = () => QuantumTunnel.TunnelAsync(
            () => throw new InvalidOperationException("retained by classical authority"),
            fallback: -1,
            tunnelingProbability: 0.0,
            randomProvider: randomProvider);

        // assert
        await act.Should().ThrowAsync<InvalidOperationException>("tunnelling probability of 0 never succeeds, regardless of quantum optimism");
    }

    [Fact]
    public async Task TunnelAsync_Void_ThrowingOperation_CompletesNormally()
    {
        // arrange
        var randomProvider = Substitute.For<IRandomProvider>();
        randomProvider.GetDouble().Returns(0.0);

        // act — void tunnel over a throwing lambda
        Func<Task> act = () => QuantumTunnel.TunnelAsync(
            () => throw new Exception("tunnelled"),
            tunnelingProbability: 1.0,
            randomProvider: randomProvider);

        // assert — no exception propagates
        await act.Should().NotThrowAsync("the void operation was successfully tunnelled");
    }

    [Fact]
    public void Tunnel_Sync_ThrowingOperation_ReturnsFallback()
    {
        // arrange
        var randomProvider = Substitute.For<IRandomProvider>();
        randomProvider.GetDouble().Returns(0.0);

        // act
        var result = QuantumTunnel.Tunnel(
            () => throw new Exception("sync barrier"),
            fallback: "tunnelled",
            tunnelingProbability: 1.0,
            randomProvider: randomProvider);

        // assert
        result.Should().Be("tunnelled", "the synchronous tunnel bypasses exception barriers just as quietly");
    }

    [Fact]
    public async Task TunnelAsync_CancellationException_IsNotTunnelled()
    {
        // arrange — cancellation tokens take precedence over quantum physics
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        // act
        Func<Task> act = () => QuantumTunnel.TunnelAsync<int>(
            async () => { await Task.Delay(100, cts.Token); return 0; },
            fallback: -1,
            tunnelingProbability: 1.0,
            cancellationToken: cts.Token);

        // assert — cancellation is not a barrier; it is a directive
        await act.Should().ThrowAsync<OperationCanceledException>("cancellation is not subject to quantum tunnelling");
    }
}
