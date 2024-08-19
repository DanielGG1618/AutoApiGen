using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AutoApiGen.Wrappers;

internal class EndpointAttributeSyntax
{
    private readonly Route _route;
    private readonly string _name;

    public string? BaseRoute =>
        _route.BaseRoute;

    public static EndpointAttributeSyntax Wrap(AttributeSyntax attribute) =>
        IsValid(attribute)
            ? new(
                Route.Parse(
                    attribute.ArgumentList?.Arguments[0].Expression
                        is LiteralExpressionSyntax literalExpression
                        ? literalExpression.Token.ValueText
                        : ""
                ),
                attribute.Name.ToString()
            )
            : throw new ArgumentException("Provided attribute is not valid Endpoint Attribute");

    public static bool IsValid(AttributeSyntax attribute) =>
        StaticData.EndpointAttributeNames.Contains(attribute.Name.ToString());

    public string GetRelationalRoute() =>
        _route.GetRelationalRoute();

    public string GetHttpMethod() =>
        _name.Remove(_name.Length - "Endpoint".Length);

    public IEnumerable<RoutePart.ParameterRoutePart> GetRouteParameters() =>
        _route.GetParameters();

    private EndpointAttributeSyntax(Route route, string name) =>
        (_route, _name) = (route, name);
}
