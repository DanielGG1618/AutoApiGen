using System.Globalization;
using AutoApiGen.Extensions;
using AutoApiGen.Models;
using Microsoft.CodeAnalysis;

namespace AutoApiGen.Templates;

internal static class ParameterTemplate
{
    internal readonly record struct Data(
        string Attributes,
        string Type,
        string Name,
        string? Default
    ) : ITemplateData
    {
        public static Data FromRoute(RoutePart.ParameterRoutePart parameter) => new(
            Attributes: "[global::Microsoft.AspNetCore.Mvc.FromRoute] ",
            parameter.Type ?? "string",
            parameter.Name,
            parameter.Default
        );

        public static Data FromSymbol(IParameterSymbol parameter) => new(
            Attributes: "",
            parameter.Type.ToString(),
            parameter.Name,
            parameter.HasExplicitDefaultValue ? parameter.ExplicitDefaultValue switch
            {
                string str => $"\"{str}\"",
                char ch => $"'{ch}'",
                bool b => b ? "true" : "false",
                float f => f.ToString(CultureInfo.InvariantCulture) + 'f',
                double d => d.ToString(CultureInfo.InvariantCulture) + 'd',
                decimal d => d.ToString(CultureInfo.InvariantCulture) + 'm',
                null => "null!",
                _ => parameter.ExplicitDefaultValue.ToString(),
            } : null
        );
    }

    internal static string Render(Data data) =>
        $"{data.Attributes}{data.Type} {data.Name}{data.Default.ApplyIfNotNullOrEmpty(static def => $" = {def}")}";
}
