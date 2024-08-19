using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AutoApiGen.Models;

internal readonly record struct EndpointAttributeModel
{
    private readonly Route _route;
    private readonly string _name;

    public string? BaseRoute =>
        _route.BaseRoute;

    public static EndpointAttributeModel Create(AttributeData attribute) =>
        !IsValid(attribute) ? throw new ArgumentException("Provided attribute is not valid Endpoint Attribute")
            : new(
                Route.Parse(attribute.ConstructorArguments.FirstOrDefault().Value?.ToString() ?? ""),
                attribute.AttributeClass!.Name
            );

    public static bool IsValid(AttributeData attribute) =>
        StaticData.EndpointAttributeNames.Contains(attribute.AttributeClass?.Name ?? "");
       
    public static bool IsValid(AttributeSyntax attribute) =>
        StaticData.EndpointAttributeNames.Contains(attribute.Name.ToString());

    public string GetRelationalRoute() =>
        _route.GetRelationalRoute();

    public string GetHttpMethod() =>
        _name.Remove(_name.Length - "Endpoint".Length);

    public IEnumerable<RoutePart.ParameterRoutePart> GetRouteParameters() =>
        _route.GetParameters();

    private EndpointAttributeModel(Route route, string name) =>
        (_route, _name) = (route, name);
}
