using ProcrastiN8.Services;

namespace ProcrastiN8.Tests.Services;

public class TaskServiceTests
{
    [Fact]
    public void MarkTaskDone_DoesNotThrow()
    {
        // Arrange
        var service = new TaskService();

        // Act
        var ex1 = Record.Exception(() => service.MarkTaskDone(true));
        var ex2 = Record.Exception(() => service.MarkTaskDone(false));

        // Assert
        ex1.Should().BeNull();
        ex2.Should().BeNull();
    }
}