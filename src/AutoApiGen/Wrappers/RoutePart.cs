using AutoApiGen.Extensions;

namespace AutoApiGen.Wrappers;

/*[Closed]*/
public abstract record RoutePart
{
    public sealed record LiteralRoutePart(string Value) : RoutePart;
    public sealed record RawParameterRoutePart(string Name, string? Type = null, string? Default = null) : RoutePart;
    public sealed record OptionalParameterRoutePart(string Name, string? Type = null) : RoutePart;
    public sealed record CatchAllParameterRoutePart(string Name, string? Type = null, string? Default = null) : RoutePart;
    
    public static IImmutableList<RoutePart> Parse(IEnumerable<string> parts) =>
        parts.Select(Parse).ToImmutableArray();

    private static RoutePart Parse(string part)
    {
        if (!(part.StartsWith("{") && part.EndsWith("}")))
            return new LiteralRoutePart(part);

        switch (part.ToList())
        {
            case [.., '?', _, _]:
                return part.Strip(front: 1, back: 2).Split(':').ToList() switch
                {
                    [var name] => new OptionalParameterRoutePart(name),
                    [var name, var type] => new OptionalParameterRoutePart(name, type),
                    _ => throw new ArgumentException()
                };
            case [_, '*', ..]:
            {
                var defaultSplit = part
                    .Strip(front: 1, back: 2)
                    .Split('=');

                var typeSplit = defaultSplit[0].Split(':');

                return new CatchAllParameterRoutePart(
                    Name: typeSplit[0],
                    Type: typeSplit.Length > 1 ? typeSplit[1] : null,
                    Default: defaultSplit.Length > 1 ? defaultSplit[1] : null
                );
            }
            default:
            {
                var defaultSplit = part
                    .Strip(front: 1, back: 1)
                    .Split('=');

                var typeSplit = defaultSplit[0].Split(':');

                return new RawParameterRoutePart(
                    Name: typeSplit[0],
                    Type: typeSplit.Length > 1 ? typeSplit[1] : null,
                    Default: defaultSplit.Length > 1 ? defaultSplit[1] : null
                );
            }
        }
    }
}
