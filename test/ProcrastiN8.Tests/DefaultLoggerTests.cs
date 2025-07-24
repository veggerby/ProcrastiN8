namespace ProcrastiN8.Tests;

public class DefaultLoggerTests
{
    [Fact]
    public void Debug_DoesNotThrow()
    {
        // Arrange
        var logger = new DefaultLogger();

        // Act
        var ex = Record.Exception(() =>
        {
            logger.Debug("test {0}", 1);
        });

        // Assert
        ex.Should().BeNull();
    }

    [Fact]
    public void Error_DoesNotThrow()
    {
        // Arrange
        var logger = new DefaultLogger();

        // Act
        var ex = Record.Exception(() =>
        {
            logger.Error("error {0}", 2);
        });

        // Assert
        ex.Should().BeNull();
    }

    [Fact]
    public void Info_DoesNotThrow()
    {
        // Arrange
        var logger = new DefaultLogger();

        // Act
        var ex = Record.Exception(() =>
        {
            logger.Info("info {0}", 3);
        });

        // Assert
        ex.Should().BeNull();
    }
}