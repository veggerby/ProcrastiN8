using ProcrastiN8.Services;

namespace ProcrastiN8.Tests.Services;

public class RetryServiceTests
{
    [Fact]
    public async Task RetryUntilDone_SucceedsOnFirstTry()
    {
        // Arrange
        var service = new RetryService();
        int count = 0;

        // Act
        var result = await service.RetryUntilDone(() => { count++; return Task.FromResult(42); }, 3);

        // Assert
        result.Should().Be(42);
        count.Should().Be(1);
    }

    [Fact]
    public async Task RetryUntilDone_RetriesAndThrows()
    {
        // Arrange
        var service = new RetryService();
        int count = 0;

        // Act
        Func<Task> act = async () =>
        {
            await service.RetryUntilDone<int>(() => { count++; throw new InvalidOperationException(); }, 2);
        };

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>();
        count.Should().Be(2);
    }

    [Fact]
    public async Task RetryUntilDone_Throws_When_MaxAttempts_Is_Invalid()
    {
        // Arrange
        var service = new RetryService();

        // Act
        Func<Task> act = async () => await service.RetryUntilDone(() => Task.FromResult(1), 0);

        // Assert
        await act.Should().ThrowAsync<ArgumentOutOfRangeException>();
    }

    [Fact]
    public async Task RetryUntilDone_Respects_Cancellation()
    {
        // Arrange
        var service = new RetryService();
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act
        Func<Task> act = async () => await service.RetryUntilDone(() => Task.FromResult(1), 3, cts.Token);

        // Assert
        await act.Should().ThrowAsync<OperationCanceledException>();
    }
}
