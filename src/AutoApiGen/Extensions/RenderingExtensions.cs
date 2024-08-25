using System.Collections.Immutable;

namespace AutoApiGen.Extensions;

internal static class RenderingExtensions
{
    public static string RenderAndJoin<T>(
        this List<T> datas,
        Func<T, string> renderer,
        string separator = "\n"
    ) => string.Join(separator, datas.Select(renderer));

    public static string RenderAndJoin<T>(
        this T[] datas,
        Func<T, string> renderer,
        string separator = "\n"
    ) => string.Join(separator, datas.Select(renderer));
    
    public static string RenderAndJoin<T>(
        this ImmutableArray<T> datas,
        Func<T, string> renderer,
        string separator = "\n"
    ) => string.Join(separator, datas.Select(renderer));
}
