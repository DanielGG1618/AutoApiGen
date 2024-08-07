using AutoApiGen.Extensions;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AutoApiGen.Wrappers;

public class EndpointAttributeSyntax
{
    private readonly string _name;
    private readonly RouteString _route;

    public string BaseRoute =>
        _route.BaseRoute;

    public static EndpointAttributeSyntax Wrap(AttributeSyntax attribute) =>
        IsValid(attribute)
            ? new(
                route: RouteString.Wrap(
                    attribute.ArgumentList?.Arguments
                        .First().Expression is LiteralExpressionSyntax literalExpression
                        ? literalExpression.Token.ValueText
                        : ""
                ),
                name: attribute.Name.ToString()
            )
            : throw new InvalidOperationException("Provided attribute is not valid Endpoint Attribute");
    
    public static bool IsValid(AttributeSyntax attribute) =>
        StaticData.EndpointAttributeNames.Contains(attribute.Name.ToString());
    
    public string GetRelationalRoute() => 
        _route.GetRelationalRoute();
    
    public string GetHttpMethod() =>
        _name.Remove(_name.Length - "Endpoint".Length);

    private EndpointAttributeSyntax(RouteString route, string name) =>
        (_route, _name) = (route, name);
}
