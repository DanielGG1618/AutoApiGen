namespace AutoApiGen.Wrappers;

public class RouteString
{
    private readonly string _string;
    private readonly IImmutableList<string> _parts;

    public string BaseRoute => _parts[0];

    public static RouteString Wrap(string @string) => new(
        @string,
        parts: @string.Split('/').ToImmutableArray()
    );

    public string GetRelationalRoute() =>
        string.Join(separator: "/", _parts.Skip(1));
    
    public static implicit operator string(RouteString routeString) => routeString._string;

    private RouteString(string @string, IImmutableList<string> parts) => 
        (_string, _parts) = (@string, parts);
}
