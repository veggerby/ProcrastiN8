using ProcrastiN8.Common;
using ProcrastiN8.Extensions;

namespace ProcrastiN8.Tests.Extensions;

public class StringExcusesTests
{
    [Fact]
    public void ToExcuse_Returns_Default_Excuse_When_No_Provider()
    {
        // arrange
        var context = "Unit tests";

        // act
        var excuse = context.ToExcuse();

        // assert
        excuse.Should().Be($"Unable to process '{context}' due to unforeseen circumstances.", "default excuses are the backbone of plausible deniability");
    }

    [Fact]
    public void ToExcuse_Uses_ExcuseProvider_When_Provided()
    {
        // arrange
        var context = "Integration tests";
        var provider = Substitute.For<IExcuseProvider>();
        provider.GetExcuse().Returns("The CI pipeline is on a coffee break.");

        // act
        var excuse = context.ToExcuse(provider);

        // assert
        excuse.Should().Contain("The CI pipeline is on a coffee break.", "injected excuses are the gold standard of accountability avoidance");
    }

    [Fact]
    public void ToExcuseWithPrefix_Prepends_Prefix_And_Uses_Provider()
    {
        // arrange
        var context = "Documentation";
        var prefix = "[BLOCKED] ";
        var provider = Substitute.For<IExcuseProvider>();
        provider.GetExcuse().Returns("Documentation is still being written by Schrödinger's intern.");

        // act
        var excuse = context.ToExcuseWithPrefix(prefix, provider);

        // assert
        excuse.Should().StartWith(prefix, "prefixes add gravitas to even the most questionable excuses");
        excuse.Should().Contain("Documentation is still being written by Schrödinger's intern.");
    }
}
