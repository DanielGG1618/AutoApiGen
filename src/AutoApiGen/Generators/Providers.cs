using System.Collections.Immutable;
using AutoApiGen.ConfigAttributes;
using AutoApiGen.Extensions;
using AutoApiGen.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AutoApiGen.Generators;

internal static class Providers
{
    public static IncrementalValuesProvider<string> CreateMediatorPackageNameProvider(
        this SyntaxValueProvider syntaxValueProvider
    ) => syntaxValueProvider.CreateSyntaxProvider(
        predicate: static (node, _) =>
            node is AttributeSyntax { ArgumentList.Arguments.Count: 1 } attribute
            && attribute.Name.ToString().Contains(nameof(SetMediatorPackageAttribute)[..^"Attribute".Length]),

        transform: static (syntaxContext, _) =>
            syntaxContext.Node is AttributeSyntax attribute
            && attribute.ArgumentList?.Arguments[0].Expression is LiteralExpressionSyntax expression
                ? expression.Token.ValueText : StaticData.DefaultMediatorPackageName
    );

    public static IncrementalValuesProvider<ResultTypeConfig?> CreateResultTypeConfigProvider(
        this SyntaxValueProvider syntaxValueProvider
    ) => syntaxValueProvider.CreateSyntaxProvider(
        predicate: static (node, _) =>
            node is AttributeSyntax { ArgumentList.Arguments.Count: >= 1 } attribute
            && attribute.Name.ToString().Contains(nameof(ResultTypeConfigurationAttribute)[..^"Attribute".Length]),

        transform: static (syntaxContext, _) =>
            Method((AttributeSyntax)syntaxContext.Node)
    );

    private static ResultTypeConfig? Method(AttributeSyntax attribute)
    {
        var arguments = attribute.ArgumentList?.Arguments
            .Where(arg => arg.NameEquals is not null)
            .ToImmutableDictionary(
                keySelector: arg =>
                    arg.NameEquals!.Name.Identifier.Text,
                elementSelector: arg =>
                    arg.Expression is LiteralExpressionSyntax literal ? literal.Token.ValueText : null
            );
        
        return arguments is null ? null
            : new ResultTypeConfig(
                TypeName: arguments[nameof(ResultTypeConfigurationAttribute.TypeName)]
                          ?? throw new ArgumentException("TypeName is missing"),
                MatchMethodName: arguments[nameof(ResultTypeConfigurationAttribute.MatchMethodName)]
                                 ?? "Match",
                ErrorHandlerMethod(arguments)
            );

        (string Name, string Implementation)? ErrorHandlerMethod(ImmutableDictionary<string, string?> args) =>
            args[nameof(ResultTypeConfigurationAttribute.ErrorHandlerMethodName)] is string name
            && args[nameof(ResultTypeConfigurationAttribute.ErrorHandlerMethodImplementation)] is string implementation
                ? (
                    name,
                    implementation
                ) 
                : null;
    }

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
