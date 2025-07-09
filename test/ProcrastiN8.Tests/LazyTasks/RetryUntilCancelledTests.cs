using ProcrastiN8.LazyTasks;

namespace ProcrastiN8.Tests.LazyTasks;

public class RetryUntilCancelledTests
{
    [Fact]
    public async Task RunForever_SucceedsOnFirstTry()
    {
        // Arrange
        int count = 0;

        // Act
        await RetryUntilCancelled.RunForever(() => { count++; return Task.CompletedTask; }, initialDelay: TimeSpan.FromMilliseconds(10));

        // Assert
        count.Should().Be(1);
    }

    [Fact]
    public async Task RunForever_RetriesAndThrowsOnMaxRetries()
    {
        // Arrange
        int count = 0;

        // Act
        Func<Task> act = async () =>
        {
            await RetryUntilCancelled.RunForever(() => { count++; throw new InvalidOperationException(); }, initialDelay: TimeSpan.FromMilliseconds(10), maxRetries: 2);
        };

        // Assert
        await act.Should().ThrowAsync<RetryUntilCancelled.RetryExhaustedException>();
        count.Should().Be(2);
    }
}