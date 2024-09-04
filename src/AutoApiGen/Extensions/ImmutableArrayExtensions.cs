using System.Collections.Immutable;
using System.Diagnostics.Contracts;

namespace AutoApiGen.Extensions;

internal static class ImmutableArrayExtensions
{
    [Pure]
    public static bool EqualsSequentially<T>(this ImmutableArray<T>? array, ImmutableArray<T>? other)
        where T : IEquatable<T> => (array, other) switch
    {
        (null, null) => true,
        (not null, not null) => array.Value.AsSpan().SequenceEqual(other.Value.AsSpan()),
        _ => false
    };
    
    [Pure]
    public static bool EqualsSequentially<T>(this ImmutableArray<T> array, ImmutableArray<T>? other)
        where T : IEquatable<T> =>
        other.HasValue
        && array.AsSpan().SequenceEqual(other.Value.AsSpan());

    [Pure]
    public static bool EqualsSequentially<T>(this ImmutableArray<T> array, ImmutableArray<T> other)
        where T : IEquatable<T> =>
        array.AsSpan().SequenceEqual(other.AsSpan());
}
