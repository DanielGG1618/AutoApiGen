using System.Collections.Immutable;
using AutoApiGen.Extensions;
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

    public bool Equals(TypeModel other) =>
        Name == other.Name
        && FullName == other.FullName
        && TypeArguments.EqualsSequentially(other.TypeArguments);

    public override int GetHashCode()
    {
        unchecked
        {
            int hashCode = Name.GetHashCode();
            hashCode = (hashCode * 397) ^ FullName.GetHashCode();
            hashCode = (hashCode * 397) ^ TypeArguments.GetHashCode();
            return hashCode;
        }
    }

    private TypeModel(string name, string fullName, ImmutableArray<TypeModel>? typeArguments) =>
        (Name, FullName, TypeArguments) = (name, fullName, typeArguments);
}
