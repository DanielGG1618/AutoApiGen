using System.Collections.Immutable;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AutoApiGen.Generators;

internal readonly record struct ResultTypeConfig(
    string TypeName,
    string MatchMethodName,
    (string Name, string Implementation)? ErrorHandlerMethod
)
{
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static ResultTypeConfig? TryCreate(AttributeSyntax attribute)
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
                TypeName: arguments["TypeName"]
                ?? throw new ArgumentException("TypeName is missing"),
                MatchMethodName: arguments["MatchMethodName"]
                ?? "Match",
                ErrorHandlerMethod(arguments)
            );

        (string Name, string Implementation)? ErrorHandlerMethod(ImmutableDictionary<string, string?> args) =>
            args["ErrorHandlerMethodName"] is string name
            && args["ErrorHandlerMethodImplementation"] is string implementation
                ? (
                    name,
                    implementation
                ) 
                : null;
    }
}
