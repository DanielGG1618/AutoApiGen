using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace AutoApiGen.Models;

internal readonly record struct TypeModel
{
    public string Name { get; }
    public string FullName { get; }
    public ImmutableArray<TypeModel>? TypeArguments { get; }
    
    public static TypeModel FromSymbol(ITypeSymbol symbol) => new(
        symbol.Name,
        fullName: symbol.ToString().TrimEnd('?'),
        typeArguments: symbol is INamedTypeSymbol namedType
            ? namedType.TypeArguments.Select(FromSymbol).ToImmutableArray() : null
    );
    
    private TypeModel(string name, string fullName, ImmutableArray<TypeModel>? typeArguments) =>
        (Name, FullName, TypeArguments) = (name, fullName, typeArguments);
}
