using ProcrastiN8.Services;

namespace ProcrastiN8.Tests.Services;

public class TaskServiceTests
{
    [Fact]
    public void MarkTaskDone_DoesNotThrow()
    {
        // arrange
        var service = new TaskService();

        // act
        var ex1 = Record.Exception(() => service.MarkTaskDone(true));
        var ex2 = Record.Exception(() => service.MarkTaskDone(false));

        // assert
        ex1.Should().BeNull();
        ex2.Should().BeNull();
    }
}