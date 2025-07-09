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
}