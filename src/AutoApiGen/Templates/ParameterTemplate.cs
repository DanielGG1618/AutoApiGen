using System.CodeDom.Compiler;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
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

    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static ParameterTemplate FromModel(ParameterModel parameter) => new(
        AttributesFor(parameter),
        parameter.Type,
        parameter.Name.WithLowerFirstLetter(),
        parameter.Default
    );

    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    private static string AttributesFor(in ParameterModel parameter) => parameter.Source switch
    {
        From.Route => "[global::Microsoft.AspNetCore.Mvc.FromRoute] ",
        From.Query => "[global::Microsoft.AspNetCore.Mvc.FromQuery] ",
        From.Body => "[global::Microsoft.AspNetCore.Mvc.FromBody] ",
        From.Form => parameter.Type.EndsWith("IFileForm")
            ? "[global::Microsoft.AspNetCore.Mvc.FromForm] "
            : "",
        _ => throw new ArgumentOutOfRangeException(nameof(parameter.Source))
    };
}
