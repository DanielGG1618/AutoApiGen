using System.Collections.Immutable;

namespace AutoApiGen.Models;

internal readonly record struct Route
{
    public string? BaseRoute { get; }
    public string RelationalRoute { get; }
    public ImmutableArray<RoutePart.ParameterRoutePart> Parameters { get; }

    public static Route Parse(string value)
    {
        var parts = value.Split('/')
            .Select(RoutePart.Parse)
            .ToArray();

        var baseRoute = parts[0] is RoutePart.LiteralRoutePart(var literal) ? literal : null;

        return new Route(
            baseRoute,
            string.Join(separator: "/", parts.Skip(baseRoute is null ? 0 : 1).Select(RoutePart.Format)),
            parts.OfType<RoutePart.ParameterRoutePart>().ToImmutableArray()
        );
    }

    private Route(string? baseRoute, string relationalRoute, ImmutableArray<RoutePart.ParameterRoutePart> parameters) =>
        (BaseRoute, RelationalRoute, Parameters) =
        (baseRoute, relationalRoute, parameters);
}
