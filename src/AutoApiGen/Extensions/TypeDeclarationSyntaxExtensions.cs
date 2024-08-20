using System.Collections.Immutable;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AutoApiGen.Extensions;

internal static class TypeDeclarationSyntaxExtensions
{
    public static IEnumerable<AttributeSyntax> GetAttributes(this TypeDeclarationSyntax type) =>
        type.AttributeLists.SelectMany(list => list.Attributes);

    public static bool HasAttributeWithNameFrom(
        this TypeDeclarationSyntax type,
        IImmutableSet<string> names
    ) => type.GetAttributes().ContainsAttributeWithNameFrom(names);

    public static bool HasAttributeWithNameFrom(
        this TypeDeclarationSyntax type,
        IImmutableSet<string> names,
        out AttributeSyntax attribute
    ) => type.GetAttributes().ContainsAttributeWithNameFrom(names, out attribute);
}
