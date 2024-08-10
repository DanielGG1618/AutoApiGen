using AutoApiGen.Exceptions;

namespace AutoApiGen.Wrappers;

public class Route
{
    private readonly IImmutableList<RoutePart> _parts;

    public string BaseRoute => _parts[0] is RoutePart.LiteralRoutePart(var value) ? value : "TODO";

    public string GetRelationalRoute() =>
        string.Join(separator: "/", _parts.Skip(1).Select(RoutePartFormatter.Format));

    public static Route Wrap(string value) => 
        new(RoutePart.Parse(value.Split('/')));

    private Route(IImmutableList<RoutePart> parts) => 
        _parts = parts;
}

public class RoutePartFormatter
{
    public static string Format(RoutePart part) => part switch
    {
        RoutePart.LiteralRoutePart(var value) => value,
        
        RoutePart.RawParameterRoutePart(var name, var type, var defaultValue) =>
            $$"""{{{FormatName(name)}}{{FormatType(type)}}{{FormatDefault(defaultValue)}}}""",
        
        RoutePart.OptionalParameterRoutePart(var name, var type) => 
            $$"""{{{FormatName(name)}}{{FormatType(type)}}}""",
        
        RoutePart.CatchAllParameterRoutePart(var name, var type, var defaultValue) =>    
            $$"""{{{FormatName(name)}}{{FormatType(type)}}{{FormatDefault(defaultValue)}}}""",
        
        _ => throw new ThisIsUnionException()
    };

    private static string FormatName(string name) => name;
    private static string FormatType(string? type) => type is null ? "" : $":{type}";
    private static string FormatDefault(string? defaultValue) => defaultValue is null ? "" : $"={defaultValue}";
}