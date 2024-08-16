using System.Collections.Immutable;

namespace AutoApiGen.Wrappers;

public class Route
{
    private readonly ImmutableArray<RoutePart> _parts;

    public static Route Wrap(string value) => new
    (
        value.Split('/')
            .Select(RoutePart.Parse)
            .ToImmutableArray()
    );

    public string BaseRoute => _parts[0] is RoutePart.LiteralRoutePart(var value) ? value : "TODO";

    public string GetRelationalRoute() =>
        string.Join(separator: "/", _parts.Skip(1).Select(RoutePart.Format));

    public IEnumerable<RoutePart.ParameterRoutePart> GetParameters() =>
        _parts.OfType<RoutePart.ParameterRoutePart>();

    private Route(ImmutableArray<RoutePart> parts) =>
        _parts = parts;
}
