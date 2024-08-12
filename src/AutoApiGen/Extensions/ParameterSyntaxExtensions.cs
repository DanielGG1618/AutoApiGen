using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AutoApiGen.Extensions;

internal static class ParameterSyntaxExtensions
{
    public static string Name(this ParameterSyntax parameter) =>
        parameter.Identifier.Text;
}
