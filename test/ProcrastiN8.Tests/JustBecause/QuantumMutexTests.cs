using ProcrastiN8.JustBecause;

namespace ProcrastiN8.Tests.JustBecause;

public class QuantumMutexTests
{
    [Fact]
    public async Task AcquireAsync_SingleThread_Succeeds()
    {
        // arrange
        using var mutex = new QuantumMutex();

        // act — acquiring the lock should never block for the acquiring thread
        using var handle = await mutex.AcquireAsync();

        // assert
        mutex.SimultaneousHolders.Should().Be(1, "exactly one thread holds this particular version of exclusivity");
    }

    [Fact]
    public void AcquireAsync_MultipleThreads_AllSucceed_Simultaneously()
    {
        // arrange — use dedicated Thread objects to guarantee unique thread IDs
        using var mutex = new QuantumMutex();
        const int threadCount = 5;
        var allHolding = new CountdownEvent(threadCount);
        var release = new ManualResetEventSlim(false);
        var handles = new IDisposable?[threadCount];
        var exceptions = new Exception?[threadCount];

        var threads = Enumerable.Range(0, threadCount).Select(i => new Thread(() =>
        {
            try
            {
                handles[i] = mutex.AcquireAsync().GetAwaiter().GetResult();
                allHolding.Signal(); // signal that this thread holds the lock
                release.Wait();     // wait until the assertion is complete
            }
            catch (Exception ex)
            {
                exceptions[i] = ex;
            }
        })).ToList();

        // act — start all threads
        foreach (var t in threads) { t.Start(); }
        allHolding.Wait(); // wait until all threads hold their personal lock

        // assert — all threads hold their exclusive lock simultaneously (quantum confirmed)
        mutex.SimultaneousHolders.Should().Be(threadCount,
            "every thread holds its own valid universe's lock simultaneously — this is the intended behaviour");

        // cleanup
        release.Set();
        foreach (var t in threads) { t.Join(); }
        foreach (var h in handles) { h?.Dispose(); }

        exceptions.Should().OnlyContain(e => e == null, "no thread should have encountered an error acquiring the lock");
    }

    [Fact]
    public async Task Dispose_Handle_DecrementsHolderCount()
    {
        // arrange
        using var mutex = new QuantumMutex();
        var handle = await mutex.AcquireAsync();

        // act
        handle.Dispose();

        // assert
        mutex.SimultaneousHolders.Should().Be(0, "releasing the lock returns the holder count to zero — for now");
    }

    [Fact]
    public async Task AcquireAsync_AfterDispose_ThrowsObjectDisposedException()
    {
        // arrange
        var mutex = new QuantumMutex();
        mutex.Dispose();

        // act
        Func<Task> act = () => mutex.AcquireAsync();

        // assert
        await act.Should().ThrowAsync<ObjectDisposedException>("the mutex cannot be acquired after it has been disposed, unlike most real-world concurrency policies");
    }

    [Fact]
    public async Task Handle_DisposeCalledTwice_DoesNotThrow()
    {
        // arrange
        using var mutex = new QuantumMutex();
        var handle = await mutex.AcquireAsync();

        // act — dispose twice (idempotent)
        handle.Dispose();
        Action act = handle.Dispose;

        // assert
        act.Should().NotThrow("double-disposal of a quantum lock handle is tolerated, unlike double-booking a conference room");
    }
}
