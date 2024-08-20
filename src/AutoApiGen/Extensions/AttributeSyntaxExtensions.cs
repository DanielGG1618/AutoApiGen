using System.Collections.Immutable;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AutoApiGen.Extensions;

internal static class AttributeSyntaxExtensions
{
    public static bool ContainsAttributeWithNameFrom(
        this IEnumerable<AttributeSyntax> attributes,
        IImmutableSet<string> names
    ) => attributes.Any(attribute => names.Contains(attribute.Name.ToString()));
    
    public static bool ContainsAttributeWithNameFrom(
        this IEnumerable<AttributeSyntax> attributes,
        IImmutableSet<string> names,
        out AttributeSyntax attribute
    ) => (attribute =
            attributes.FirstOrDefault(attribute =>
                names.Contains(attribute.Name.ToString())
            )!
        ) is not null;
}
