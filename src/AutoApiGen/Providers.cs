using AutoApiGen.Extensions;
using AutoApiGen.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AutoApiGen;

internal static class Providers
{
    public static IncrementalValuesProvider<string> CreateMediatorPackageNameProvider(
        this SyntaxValueProvider syntaxValueProvider
    ) => syntaxValueProvider.CreateSyntaxProvider(
        predicate: static (node, _) =>
            node is AttributeSyntax { ArgumentList.Arguments.Count: 1 } attribute
            && attribute.Name.ToString().Contains("SetMediatorPackage"),

        transform: static (syntaxContext, _) =>
            syntaxContext.Node is AttributeSyntax attribute
            && attribute.ArgumentList?.Arguments[0].Expression is LiteralExpressionSyntax expression
                ? expression.Token.ValueText : "global::MediatR"
    );

    public static IncrementalValuesProvider<EndpointContractModel> CreateEndpointsProvider(
        this SyntaxValueProvider syntaxValueProvider
    ) => syntaxValueProvider.CreateSyntaxProvider(
        predicate: static (node, _) =>
            node is TypeDeclarationSyntax { AttributeLists.Count: > 0 } type
            && type.HasAttributeWithNameFrom(StaticData.EndpointAttributeNames, out var attribute)
            && EndpointAttributeModel.IsValid(attribute)
            && EndpointContractModel.IsValid(type),

        transform: static (syntaxContext, _) => EndpointContractModel.Create(
            (INamedTypeSymbol)syntaxContext.SemanticModel.GetDeclaredSymbol(syntaxContext.Node)!
        )
    );
}
