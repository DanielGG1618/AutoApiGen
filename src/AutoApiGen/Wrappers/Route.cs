using System.Collections.Immutable;

namespace AutoApiGen.Wrappers;

internal class Route
{
    private readonly ImmutableArray<RoutePart> _parts;

    public static Route Parse(string value) => new
    (
        value.Split('/')
            .Select(RoutePart.Parse)
            .ToImmutableArray()
    );

    public string? BaseRoute => _parts[0] is RoutePart.LiteralRoutePart(var value) ? value : null;

    public string GetRelationalRoute() =>
        string.Join(separator: "/", _parts.Skip(BaseRoute is null ? 0 : 1).Select(RoutePart.Format));

    public IEnumerable<RoutePart.ParameterRoutePart> GetParameters() =>
        _parts.OfType<RoutePart.ParameterRoutePart>();

    private Route(ImmutableArray<RoutePart> parts) =>
        _parts = parts;
}
