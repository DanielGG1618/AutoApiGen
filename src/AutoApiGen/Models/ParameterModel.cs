using System.Diagnostics.Contracts;
using System.Globalization;
using Microsoft.CodeAnalysis;

namespace AutoApiGen.Models;

internal readonly record struct ParameterModel(
    From Source,
    string Type,
    string Name,
    string? Default
)
{
    [Pure]
    public static ParameterModel FromRoute(RoutePart.ParameterRoutePart parameter) => new(
        Source: From.Route,
        parameter.Type ?? "string",
        parameter.Name,
        parameter.Default
    );

    [Pure]
    public static ParameterModel FromSymbol(IParameterSymbol parameter) => new(
        SourceOf(parameter),
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

    [Pure]
    private static From SourceOf(IParameterSymbol parameter)
    {
        if (parameter.Type.ToString().Contains("IFormFile"))
            return From.Form;
        
        foreach (var attribute in parameter.GetAttributes())
        {
            switch (attribute.AttributeClass?.Name)
            {
                case "FromRouteAttribute": return From.Route;
                case "FromBodyAttribute": return From.Body;
                case "FromQueryAttribute": return From.Query;
                case "FromFormAttribute": return From.Form;
            }
        }

        return From.Body;
    }
    
}