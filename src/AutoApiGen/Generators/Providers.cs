using System.Diagnostics.Contracts;
using AutoApiGen.Extensions;
using AutoApiGen.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AutoApiGen.Generators;

internal static class Providers
{
    [Pure]
    public static IncrementalValuesProvider<string> CreateMediatorPackageNameProvider(
        this SyntaxValueProvider syntaxValueProvider
    ) => syntaxValueProvider.CreateSyntaxProvider(
        predicate: static (node, _) =>
            node is AttributeSyntax { ArgumentList.Arguments.Count: 1 } attribute
            && attribute.Name.ToString().Contains("SetMediatorPackage"),

        transform: static (syntaxContext, _) =>
            syntaxContext.Node is AttributeSyntax attribute
            && attribute.ArgumentList?.Arguments[0].Expression is LiteralExpressionSyntax expression
                ? expression.Token.ValueText : StaticData.DefaultMediatorPackageName
    );

    [Pure]
    public static IncrementalValuesProvider<ResultTypeConfig?> CreateResultTypeConfigProvider(
        this SyntaxValueProvider syntaxValueProvider
    ) => syntaxValueProvider.CreateSyntaxProvider(
        predicate: static (node, _) =>
            node is AttributeSyntax { ArgumentList.Arguments.Count: >= 1 } attribute
            && attribute.Name.ToString().Contains("ResultTypeConfiguration"),

        transform: static (syntaxContext, _) =>
            ResultTypeConfig.TryCreate((AttributeSyntax)syntaxContext.Node)
    );
    
    [Pure]
    public static IncrementalValuesProvider<EndpointContractModel> CreateEndpointsProvider(
        this SyntaxValueProvider syntaxValueProvider
    ) => syntaxValueProvider.CreateSyntaxProvider(
        predicate: static (node, _) =>
            node is TypeDeclarationSyntax { AttributeLists.Count: > 0 } type
            && type.HasAttributeWithNameFrom(StaticData.EndpointAttributeNames)
            && EndpointContractModel.IsValid(type),

        transform: static (syntaxContext, _) => EndpointContractModel.Create(
            (INamedTypeSymbol)syntaxContext.SemanticModel.GetDeclaredSymbol(syntaxContext.Node)!
        )
    );
}
