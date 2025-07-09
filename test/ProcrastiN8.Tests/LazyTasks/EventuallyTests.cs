using ProcrastiN8.LazyTasks;

namespace ProcrastiN8.Tests.LazyTasks;

public class EventuallyTests
{
    [Fact]
    public async Task Do_ExecutesAction()
    {
        // Arrange
        bool called = false;

        // Act
        await Eventually.Do(() => { called = true; return Task.CompletedTask; }, within: TimeSpan.FromMilliseconds(10));

        // Assert
        called.Should().BeTrue();
    }

    [Fact]
    public async Task Do_ThrowsOnException()
    {
        // Arrange
        // (no setup needed)

        // Act
        Func<Task> act = () => Eventually.Do(() => throw new InvalidOperationException(), within: TimeSpan.FromMilliseconds(10));

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>();
    }
}