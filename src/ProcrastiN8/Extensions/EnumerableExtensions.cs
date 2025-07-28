namespace ProcrastiN8.Extensions;

/// <summary>
/// Provides extension methods for <see cref="IEnumerable{T}"/> to simulate endless productivity.
/// </summary>
public static class EnumerableExtensions
{
    /// <summary>
    /// Returns an enumerable that never completes, regardless of the input.
    /// </summary>
    /// <typeparam name="T">The type of the elements.</typeparam>
    /// <param name="source">The source enumerable.</param>
    /// <returns>An infinite enumerable of the source's elements, repeated forever.</returns>
    public static IEnumerable<T> LoopForever<T>(this IEnumerable<T> source)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        while (true)
        {
            foreach (var item in source)
            {
                yield return item;
            }
        }
    }
}
