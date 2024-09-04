using System.Collections.Immutable;
using System.Diagnostics.Contracts;
using Microsoft.CodeAnalysis;

namespace AutoApiGen.Extensions;

public static class TypeSymbolExtensions
{
    [Pure]
    public static ImmutableArray<ITypeSymbol> GetTypeArgumentsOfInterfaceNamed(
        this ITypeSymbol type,
        ImmutableArray<string> interfaceNames
    ) => type.Interfaces
             .FirstOrDefault(@interface => interfaceNames.Contains(@interface.Name))?
             .TypeArguments
         ?? [];
}
