namespace Prism.Extensions;

internal static class CollectionExtensions
{
    public static void ForEach<T>(this IReadOnlyList<T> list, Action<T> action)
    {
        foreach (var item in list)
            action(item);
    }

    public static void ForEach<T>(this IList<T> list, Action<T> action)
    {
        foreach (var item in list)
            action(item);
    }
}
