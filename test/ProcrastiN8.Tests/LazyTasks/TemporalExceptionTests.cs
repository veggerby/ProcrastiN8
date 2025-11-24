using ProcrastiN8.LazyTasks;

namespace ProcrastiN8.Tests.LazyTasks;

public class TemporalExceptionTests
{
    [Fact]
    public void CausalityViolationException_Has_Default_Message()
    {
        // arrange & act
        var exception = new CausalityViolationException();

        // assert
        exception.Message.Should().Contain("Causality has been violated", "default message explains the issue");
    }

    [Fact]
    public void CausalityViolationException_Accepts_Custom_Message()
    {
        // arrange
        var message = "Time travel caused a grandfather paradox";

        // act
        var exception = new CausalityViolationException(message);

        // assert
        exception.Message.Should().Be(message, "custom message is preserved");
    }

    [Fact]
    public void CausalityViolationException_Accepts_Inner_Exception()
    {
        // arrange
        var innerException = new InvalidOperationException("Temporal core meltdown");
        var message = "Causality chain broken";

        // act
        var exception = new CausalityViolationException(message, innerException);

        // assert
        exception.Message.Should().Be(message, "message is set");
        exception.InnerException.Should().BeSameAs(innerException, "inner exception is preserved");
    }

    [Fact]
    public void TemporalWhiplashException_Has_Default_Message()
    {
        // arrange & act
        var exception = new TemporalWhiplashException();

        // assert
        exception.Message.Should().Contain("Temporal whiplash", "default message describes whiplash");
    }

    [Fact]
    public void TemporalWhiplashException_Accepts_Custom_Message()
    {
        // arrange
        var message = "Mercury retrograde caused 5 consecutive rewinds";

        // act
        var exception = new TemporalWhiplashException(message);

        // assert
        exception.Message.Should().Be(message, "custom message is preserved");
    }

    [Fact]
    public void TemporalWhiplashException_Accepts_Inner_Exception()
    {
        // arrange
        var innerException = new InvalidOperationException("Time oscillation detected");
        var message = "Whiplash from rapid time changes";

        // act
        var exception = new TemporalWhiplashException(message, innerException);

        // assert
        exception.Message.Should().Be(message, "message is set");
        exception.InnerException.Should().BeSameAs(innerException, "inner exception is preserved");
    }

    [Fact]
    public void CausalityViolationException_Is_Exception()
    {
        // arrange
        var exception = new CausalityViolationException();

        // act & assert
        exception.Should().BeAssignableTo<Exception>("it inherits from Exception");
    }

    [Fact]
    public void TemporalWhiplashException_Is_Exception()
    {
        // arrange
        var exception = new TemporalWhiplashException();

        // act & assert
        exception.Should().BeAssignableTo<Exception>("it inherits from Exception");
    }

    [Fact]
    public void CausalityViolationException_Can_Be_Thrown_And_Caught()
    {
        // arrange
        Action act = () => throw new CausalityViolationException("Test violation");

        // act & assert
        act.Should().Throw<CausalityViolationException>()
            .WithMessage("Test violation");
    }

    [Fact]
    public void TemporalWhiplashException_Can_Be_Thrown_And_Caught()
    {
        // arrange
        Action act = () => throw new TemporalWhiplashException("Test whiplash");

        // act & assert
        act.Should().Throw<TemporalWhiplashException>()
            .WithMessage("Test whiplash");
    }
}
