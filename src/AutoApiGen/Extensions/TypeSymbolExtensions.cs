using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace AutoApiGen.Extensions;

public static class TypeSymbolExtensions
{
    public static ImmutableArray<ITypeSymbol> GetTypeArgumentsOfInterfaceNamed(
        this ITypeSymbol type,
        ImmutableArray<string> interfaceNames
    ) => type.Interfaces
             .FirstOrDefault(@interface => interfaceNames.Contains(@interface.Name))?
             .TypeArguments
         ?? [];
}
