using System.Collections.Immutable;

namespace AutoApiGen.Extensions;

internal static class ImmutableArrayExtensions
{
    public static bool EqualsSequentially<T>(this ImmutableArray<T>? array, ImmutableArray<T>? other)
        where T : IEquatable<T> => (array, other) switch
    {
        (null, null) => true,
        (not null, not null) => array.Value.AsSpan().SequenceEqual(other.Value.AsSpan()),
        _ => false
    };
    
    public static bool EqualsSequentially<T>(this ImmutableArray<T> array, ImmutableArray<T>? other)
        where T : IEquatable<T> =>
        other.HasValue
        && array.AsSpan().SequenceEqual(other.Value.AsSpan());

    public static bool EqualsSequentially<T>(this ImmutableArray<T> array, ImmutableArray<T> other)
        where T : IEquatable<T> =>
        array.AsSpan().SequenceEqual(other.AsSpan());
}
