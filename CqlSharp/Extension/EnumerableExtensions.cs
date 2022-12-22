namespace CqlSharp.Extension;

internal static class EnumerableExtensions
{
    public static IEnumerable<TSource> ElementsAt<TSource>(this IEnumerable<TSource> source, IEnumerable<int> indexes)
    {
        return source.Where((_, i) => indexes.Any(index => index == i));
    }
}