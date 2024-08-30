using AutoApiGen.Extensions;
using AutoApiGen.Models;

namespace AutoApiGen.Templates;

internal static class ParameterTemplate
{
    internal readonly record struct Data(
        string Attributes,
        string Type,
        string Name,
        string? Default
    )
    {
        public static Data FromRoute(ParameterModel parameter) => new(
            Attributes: "[global::Microsoft.AspNetCore.Mvc.FromRoute] ",
            parameter.Type,
            parameter.Name.WithLowerFirstLetter(),
            parameter.Default
        );

        public static Data FromModel(ParameterModel parameter) => new(
            Attributes: "",
            parameter.Type,
            parameter.Name,
            parameter.Default
        );
    }

    internal static string Render(in Data data) =>
        $"{data.Attributes}{data.Type} {data.Name}{data.Default.ApplyIfNotNullOrEmpty(static def => $" = {def}")}";
}
