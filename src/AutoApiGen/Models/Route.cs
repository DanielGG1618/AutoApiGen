using System.Collections.Immutable;
using System.Diagnostics.Contracts;
using AutoApiGen.Extensions;

namespace AutoApiGen.Models;

internal readonly record struct Route
{
    public string? BaseRoute { get; }
    public string RelationalRoute { get; }
    public ImmutableArray<ParameterModel> Parameters { get; }

    [Pure]
    public static Route Parse(string value)
    {
        var parts = value.Trim('/').Split('/')
            .Select(RoutePart.Parse)
            .ToArray();

        var baseRoute = parts[0] is RoutePart.LiteralRoutePart(var literal) ? literal : null;

        return new Route(
            baseRoute,
            string.Join(separator: "/", parts.Skip(baseRoute is null ? 0 : 1).Select(RoutePart.Format)),
            parts.OfType<RoutePart.ParameterRoutePart>()
                .Select(ParameterModel.FromRoute)
                .ToImmutableArray()
        );
    }

    public bool Equals(Route other) =>
        BaseRoute == other.BaseRoute
        && RelationalRoute == other.RelationalRoute
        && Parameters.EqualsSequentially(other.Parameters);

    public override int GetHashCode()
    {
        unchecked
        {
            int hashCode = BaseRoute != null ? BaseRoute.GetHashCode() : 0;
            hashCode = (hashCode * 397) ^ RelationalRoute.GetHashCode();
            hashCode = (hashCode * 397) ^ Parameters.GetHashCode();
            return hashCode;
        }
    }

    private Route(string? baseRoute, string relationalRoute, ImmutableArray<ParameterModel> parameters) =>
        (BaseRoute, RelationalRoute, Parameters) =
        (baseRoute, relationalRoute, parameters);
}
