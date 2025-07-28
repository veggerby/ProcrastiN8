using ProcrastiN8.Extensions;

namespace ProcrastiN8.Tests.Extensions;

/// <summary>
/// Tests for <see cref="EnumerableExtensions"/>.
/// </summary>
public class EnumerableExtensionsTests
{
    [Fact]
    public void LoopForever_Should_Throw_On_Null_Source()
    {
        // arrange
        IEnumerable<int>? source = null;

        // act
        Action act = () => EnumerableExtensions.LoopForever(source!).ToList();

        // assert
        act.Should().Throw<ArgumentNullException>("null is the only thing that should not be repeated forever");
    }

    [Fact]
    public void LoopForever_Should_Repeat_Elements_Indefinitely()
    {
        // arrange
        var source = new[] { 1, 2, 3 };
        var enumerator = EnumerableExtensions.LoopForever(source).GetEnumerator();
        var results = new List<int>();

        // act
        for (int i = 0; i < 10; i++)
        {
            enumerator.MoveNext();
            results.Add(enumerator.Current);
        }

        // assert
        results.Should().BeEquivalentTo(new[] { 1, 2, 3, 1, 2, 3, 1, 2, 3, 1 },
            "the loop should never end, but this test must");
    }

    [Fact]
    public void LoopForever_Should_Work_With_Empty_Source()
    {
        // arrange
        var source = Array.Empty<string>();
        var enumerator = EnumerableExtensions.LoopForever(source).GetEnumerator();

        // act
        var moved = enumerator.MoveNext();

        // assert
        moved.Should().BeFalse("an empty source cannot be repeated, not even forever");
    }

    [Fact]
    public void LoopForever_Should_Enumerate_ReferenceTypeElements_Correctly()
    {
        // arrange
        var source = new[] { "alpha", "beta" };
        var enumerator = EnumerableExtensions.LoopForever(source).GetEnumerator();
        var results = new List<string>();

        // act
        for (int i = 0; i < 5; i++)
        {
            enumerator.MoveNext();
            results.Add(enumerator.Current);
        }

        // assert
        results.Should().BeEquivalentTo(new[] { "alpha", "beta", "alpha", "beta", "alpha" },
            "reference types should loop with the same relentless optimism as value types");
    }

    [Fact]
    public void LoopForever_Should_Not_Enumerate_When_Source_Is_Empty()
    {
        // arrange
        var source = Enumerable.Empty<int>();
        var enumerator = EnumerableExtensions.LoopForever(source).GetEnumerator();

        // act
        var moved = enumerator.MoveNext();

        // assert
        moved.Should().BeFalse("an empty enumerable is the only thing that can truly end");
    }

    [Fact]
    public void LoopForever_Should_Allow_Breaking_The_Loop()
    {
        // arrange
        var source = new[] { 42 };
        var enumerator = EnumerableExtensions.LoopForever(source).GetEnumerator();
        var count = 0;

        // act
        while (enumerator.MoveNext())
        {
            count++;
            if (count == 3)
            {
                break;
            }
        }

        // assert
        count.Should().Be(3, "even infinite loops can be tamed by a determined test");
    }

    [Fact]
    public void LoopForever_Should_Throw_On_Null_Source_With_ReferenceType()
    {
        // arrange
        IEnumerable<string>? source = null;

        // act
        Action act = () => EnumerableExtensions.LoopForever(source!).GetEnumerator().MoveNext();

        // assert
        act.Should().Throw<ArgumentNullException>("null reference types are not a valid source of productivity");
    }
}
