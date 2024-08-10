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

    private static RoutePart Parse(string part) => part switch
    {
        not ['{', .., '}'] => new LiteralRoutePart(part),

        ['{', .., '}'] when Regexes.RawParameterRoutePartRegex.Match(part) is { Success: true } match =>
            new RawParameterRoutePart(
                match.Groups["name"].Value,
                match.Groups["type"].Value is { Length: > 0 } type ? type : null,
                match.Groups["default"].Value is { Length: > 0 } defaultValue ? defaultValue : null
            ),

        ['{', .., '?', '}'] when Regexes.OptionalParameterRoutePartRegex.Match(part) is { Success: true } match =>
            new OptionalParameterRoutePart(
                match.Groups["name"].Value,
                match.Groups["type"].Value is { Length: > 0 } type ? type : null
            ),

        ['{', '*', .., '}'] when Regexes.CatchAllParameterRoutePartRegex.Match(part) is { Success: true } match =>
            new CatchAllParameterRoutePart(
                match.Groups["name"].Value,
                match.Groups["type"].Value is { Length: > 0 } type ? type : null,
                match.Groups["default"].Value is { Length: > 0 } defaultValue ? defaultValue : null
            ),

        _ => throw new ArgumentException("Invalid syntax", nameof(part))
    };
}
