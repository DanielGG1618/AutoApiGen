using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AutoApiGen.Models;

internal readonly record struct EndpointAttributeModel
{
    public Route Route { get; }
    public string HttpMethod { get; }

    public static EndpointAttributeModel Create(AttributeData attribute) =>
        !IsValid(attribute) ? throw new ArgumentException("Provided attribute is not valid Endpoint Attribute")
            : new(
                Route.Parse(attribute.ConstructorArguments.FirstOrDefault().Value?.ToString() ?? ""),
                attribute.AttributeClass!.Name is var name
                && name.EndsWith("Endpoint")
                    ? name[..^"Endpoint".Length]
                    : name
            );

    public static bool IsValid(AttributeData attribute) =>
        StaticData.EndpointAttributeNames.Contains(attribute.AttributeClass?.Name ?? "");
       
    public static bool IsValid(AttributeSyntax attribute) =>
        StaticData.EndpointAttributeNames.Contains(attribute.Name.ToString());
    
    private EndpointAttributeModel(Route route, string httpMethod) => 
        (Route, HttpMethod) = (route, httpMethod);
}
