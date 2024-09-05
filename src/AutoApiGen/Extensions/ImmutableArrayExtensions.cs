using System.Collections.Immutable;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace AutoApiGen.Extensions;

internal static class ImmutableArrayExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static bool EqualsSequentially<T>(this ImmutableArray<T>? array, ImmutableArray<T>? other)
        where T : IEquatable<T> => (array, other) switch
    {
        (null, null) => true,
        (not null, not null) => array.Value.AsSpan().SequenceEqual(other.Value.AsSpan()),
        _ => false
    };
    
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static bool EqualsSequentially<T>(this ImmutableArray<T> array, ImmutableArray<T>? other)
        where T : IEquatable<T> =>
        other.HasValue
        && array.AsSpan().SequenceEqual(other.Value.AsSpan());

    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static bool EqualsSequentially<T>(this ImmutableArray<T> array, ImmutableArray<T> other)
        where T : IEquatable<T> =>
        array.AsSpan().SequenceEqual(other.AsSpan());
}
