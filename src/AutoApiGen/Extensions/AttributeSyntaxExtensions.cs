using System.Collections.Immutable;
using System.Diagnostics.Contracts;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AutoApiGen.Extensions;

internal static class AttributeSyntaxExtensions
{
    [Pure]
    public static bool ContainsAttributeWithNameFrom(
        this IEnumerable<AttributeSyntax> attributes,
        ImmutableArray<string> names
    ) => attributes.Any(attribute => names.Contains(attribute.Name.ToString()));
}
