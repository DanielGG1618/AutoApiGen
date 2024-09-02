using System.CodeDom.Compiler;
using AutoApiGen.Extensions;
using AutoApiGen.Models;

namespace AutoApiGen.Templates;

internal readonly record struct ParameterTemplate(
    string Attributes,
    string Type,
    string Name,
    string? Default
)
{
    public void RenderTo(IndentedTextWriter writer) =>
        writer.Write($"{Attributes}{Type} {Name}{Default.ApplyIfNotNullOrEmpty(static def => $" = {def}")}");

    public static ParameterTemplate FromRoute(ParameterModel parameter) => new(
        Attributes: "[global::Microsoft.AspNetCore.Mvc.FromRoute] ",
        parameter.Type,
        parameter.Name.WithLowerFirstLetter(),
        parameter.Default
    );

    public static ParameterTemplate FromModel(ParameterModel parameter) => new(
        Attributes: "",
        parameter.Type,
        parameter.Name,
        parameter.Default
    );
}
