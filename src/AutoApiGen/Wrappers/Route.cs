namespace AutoApiGen.Wrappers;

public class Route
{
    private readonly IImmutableList<RoutePart> _parts;

    public string BaseRoute => _parts[0] is RoutePart.LiteralRoutePart(var value) ? value : "TODO";

    public string GetRelationalRoute() =>
        string.Join(separator: "/", _parts.Skip(1).Select(RoutePart.Format));

    public static Route Wrap(string value) => new
    (
        value.Split('/')
            .Select(RoutePart.Parse)
            .ToImmutableArray()
    );

    private Route(IImmutableList<RoutePart> parts) => 
        _parts = parts;
}
