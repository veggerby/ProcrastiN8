using ProcrastiN8.JustBecause;

namespace ProcrastiN8.Tests.JustBecause;

/// <summary>
/// Unit tests for <see cref="YAGNIValidator"/>.
/// </summary>
public class YAGNIValidatorTests
{
    [Fact]
    public void Validate_Should_AlwaysThrowNotSupportedException()
    {
        // act
        Action act = () => YAGNIValidator.Validate("Some feature");

        // assert
        act.Should().Throw<NotSupportedException>().WithMessage("*Some feature*");
    }
}
