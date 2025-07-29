using ProcrastiN8.JustBecause;

namespace ProcrastiN8.Tests.JustBecause;

/// <summary>
/// Unit tests for <see cref="RandomExceptionGenerator"/>.
/// </summary>
public class RandomExceptionGeneratorTests
{
    [Fact]
    public void GenerateException_Should_ReturnRandomException()
    {
        // arrange
        var randomProvider = Substitute.For<IRandomProvider>();
        randomProvider.GetDouble().Returns(0);
        var generator = new RandomExceptionGenerator(randomProvider, new List<Func<Exception>>
        {
            () => new InvalidOperationException("Test exception")
        });

        // act
        var exception = generator.GenerateException();

        // assert
        exception.Should().BeOfType<InvalidOperationException>();
        exception.Message.Should().Be("Test exception");
    }
}