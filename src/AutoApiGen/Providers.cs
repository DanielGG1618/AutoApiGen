using System.Collections.Immutable;
using AutoApiGen.Extensions;
using AutoApiGen.Wrappers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static AutoApiGen.StaticData;

namespace AutoApiGen;

internal static class Providers
{
    public static IncrementalValueProvider<ImmutableArray<string?>> CreateMediatorPackageNameProvider(
        this SyntaxValueProvider syntaxValueProvider
    ) => syntaxValueProvider.CreateSyntaxProvider(
        predicate: static (node, _) =>
            node is AttributeSyntax { ArgumentList.Arguments.Count: 1 } attribute
            && attribute.Name.ToString().Contains("SetMediatorPackage"),

        transform: static (syntaxContext, _) =>
            syntaxContext.Node is AttributeSyntax attribute
            && attribute.ArgumentList?.Arguments[0].Expression is LiteralExpressionSyntax expression
                ? expression.Token.ValueText : null
    ).Collect();
    
    public static IncrementalValueProvider<ImmutableArray<EndpointContractDeclarationSyntax>> CreateEndpointsProvider(
        this SyntaxValueProvider syntaxValueProvider
    ) => syntaxValueProvider.CreateSyntaxProvider(
        predicate: static (node, _) =>
            node is TypeDeclarationSyntax { AttributeLists.Count: > 0 } type
            && type.HasAttributeWithNameFrom(EndpointAttributeNames, out var attribute)
            && EndpointAttributeSyntax.IsValid(attribute)
            && EndpointContractDeclarationSyntax.IsValid(type),

        transform: static (syntaxContext, _) =>
            EndpointContractDeclarationSyntax.Wrap((TypeDeclarationSyntax)syntaxContext.Node)
    ).Collect();
}
