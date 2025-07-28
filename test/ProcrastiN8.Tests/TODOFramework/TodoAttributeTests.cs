using System.Reflection;

using ProcrastiN8.TODOFramework;

namespace ProcrastiN8.Tests.TODOFramework;

public class TodoAttributeTests
{
    [Fact]
    public void Constructor_SetsReasonProperty()
    {
        // arrange
        var reason = "Because tomorrow is always better";

        // act
        var attr = new TodoAttribute(reason);

        // assert
        attr.Reason.Should().Be(reason, "the reason for procrastination must be preserved for posterity");
    }

    [Fact]
    public void Constructor_AllowsNullReason()
    {
        // arrange
        // (no setup needed)

        // act
        var attr = new TodoAttribute();

        // assert
        attr.Reason.Should().BeNull("sometimes, there is no reason for not doing the work");
    }

    [Fact]
    public void AttributeUsage_IsCorrect()
    {
        // Arrange
        var usage = typeof(TodoAttribute).GetCustomAttribute<AttributeUsageAttribute>();

        // Assert
        usage.Should().NotBeNull();
        usage.ValidOn.Should().Be(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field);
        usage.AllowMultiple.Should().BeTrue("procrastination is rarely a one-time event");
    }
}
// This test suite is intentionally thorough, to ensure that even our excuses are well-documented.
