using System.Globalization;
using AutoApiGen.Extensions;
using Microsoft.CodeAnalysis;

namespace AutoApiGen.Models;

internal readonly record struct ParameterModel(
    string Type,
    string Name,
    string? Default
)
{
    public static ParameterModel FromRoute(RoutePart.ParameterRoutePart parameter) => new(
        parameter.Type ?? "string",
        parameter.Name.WithLowerFirstLetter(),
        parameter.Default
    );

    public static ParameterModel FromSymbol(IParameterSymbol parameter) => new(
        parameter.Type.ToString(),
        parameter.Name,
        parameter.HasExplicitDefaultValue ? parameter.ExplicitDefaultValue switch
        {
            string s => $"\"{s}\"",
            char ch => $"'{ch}'",
            bool b => b ? "true" : "false",
            float f => f.ToString(CultureInfo.InvariantCulture) + 'f',
            double d => d.ToString(CultureInfo.InvariantCulture) + 'd',
            decimal d => d.ToString(CultureInfo.InvariantCulture) + 'm',
            null => "default!",
            _ => parameter.ExplicitDefaultValue.ToString(),
        } : null
    );
}

