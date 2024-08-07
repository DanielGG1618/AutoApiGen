using Microsoft.CodeAnalysis;

namespace AutoApiGen.Extensions;

internal static class TypeSymbolExtensions
{
    public static bool IsController(this ITypeSymbol typeSymbol) =>
        typeSymbol.GetAttributes().Any(attribute =>
            attribute.AttributeClass!.Name is "ApiControllerAttribute"
        );
    
    public static IEnumerable<IPropertySymbol> Properties(this ITypeSymbol symbol) =>
        symbol
            .Members()
            .Where(member => member.Kind is SymbolKind.Property)
            .OfType<IPropertySymbol>();

    private static IEnumerable<ISymbol> Members(this ITypeSymbol type) => 
        type.WithItsBaseTypes().SelectMany(t => t.GetMembers());
    
    private static IEnumerable<ITypeSymbol> WithItsBaseTypes(this ITypeSymbol type)
    {
        var current = type;
        while (current is not null)
        {
            yield return current;
            current = current.BaseType;
        }
    }
}