using System.Collections.Immutable;
using System.Diagnostics.Contracts;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AutoApiGen.Extensions;

internal static class TypeDeclarationSyntaxExtensions
{
    [Pure]
    public static bool HasAttributeWithNameFrom(
        this TypeDeclarationSyntax type,
        ImmutableArray<string> names
    ) => type.GetAttributes().ContainsAttributeWithNameFrom(names);

    [Pure]
    private static IEnumerable<AttributeSyntax> GetAttributes(this TypeDeclarationSyntax type) =>
        type.AttributeLists.SelectMany(list => list.Attributes);
}
