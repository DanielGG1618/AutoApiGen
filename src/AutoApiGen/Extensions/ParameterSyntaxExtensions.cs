using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AutoApiGen.Extensions;

public static class ParameterSyntaxExtensions
{
    public static string Name(this ParameterSyntax parameter) =>
        parameter.Identifier.Text;
}
