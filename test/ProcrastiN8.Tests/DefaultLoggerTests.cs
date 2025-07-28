namespace ProcrastiN8.Tests;

public class DefaultLoggerTests
{
    [Fact]
    public void Debug_DoesNotThrow()
    {
        // arrange
        var logger = new DefaultLogger();

        // act
        var ex = Record.Exception(() =>
        {
            logger.Debug("test {0}", 1);
        });

        // assert
        ex.Should().BeNull();
    }

    [Fact]
    public void Error_DoesNotThrow()
    {
        // arrange
        var logger = new DefaultLogger();

        // act
        var ex = Record.Exception(() =>
        {
            logger.Error("error {0}", 2);
        });

        // assert
        ex.Should().BeNull();
    }

    [Fact]
    public void Info_DoesNotThrow()
    {
        // arrange
        var logger = new DefaultLogger();

        // act
        var ex = Record.Exception(() =>
        {
            logger.Info("info {0}", 3);
        });

        // assert
        ex.Should().BeNull();
    }
}