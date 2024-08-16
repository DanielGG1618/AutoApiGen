using AutoApiGen.Extensions;
using AutoApiGen.Wrappers;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AutoApiGen.DataObjects;

internal readonly record struct ParameterData(
    string Attributes,
    string Type,
    string Name,
    string? Default
) : ITemplateData
{
    public static ParameterData FromRoute(RoutePart.ParameterRoutePart parameter) => new(
        Attributes: "[global::Microsoft.AspNetCore.Mvc.FromRoute]",
        parameter.Type ?? "string",
        parameter.Name,
        parameter.Default
    );

    public static ParameterData FromSyntax(ParameterSyntax parameter) => new(
        Attributes: "",
        parameter.Type?.ToFullString() ?? "string",
        parameter.Name(),
        parameter.Default?.Value.ToFullString()
    );
}
