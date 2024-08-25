using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AutoApiGen.Models;

internal readonly record struct EndpointAttributeModel
{
    public Route Route { get; }
    public string HttpMethod { get; }
    public int SuccessCode { get; }
    public ImmutableArray<int> ErrorCodes { get; }

    public static EndpointAttributeModel Create(AttributeData attribute)
    {
        if (!IsValid(attribute))
            throw new ArgumentException("Provided attribute is not valid Endpoint Attribute");

        var (successCode, errorCodes) = GetStatusCodes(attribute);
        
        return new EndpointAttributeModel(
            Route.Parse(attribute.ConstructorArguments.FirstOrDefault().Value?.ToString() ?? ""),
            attribute.AttributeClass!.Name is var name
            && name.EndsWith("EndpointAttribute")
                ? name[..^"EndpointAttribute".Length] : name,
            successCode,
            errorCodes
        );
    }

    public static bool IsValid(AttributeData attribute) =>
        StaticData.EndpointAttributeNamesWithSuffix.Contains(attribute.AttributeClass?.Name ?? "");

    public static bool IsValid(AttributeSyntax attribute) =>
        StaticData.EndpointAttributeNames.Contains(attribute.Name.ToString());

    private static (int Success, ImmutableArray<int> Error) GetStatusCodes(AttributeData attribute)
    {
        var success = 200;
        var errors = new List<int>();

        foreach (var argument in attribute.NamedArguments)
            switch (argument.Key)
            {
                case "SuccessCode":
                    success = (int)argument.Value.Value!;
                    break;
                case "ErrorCodes":
                    errors.AddRange(argument.Value.Values.Select(value => (int)value.Value!));
                    break;
                case "ErrorCode":
                    errors.Add((int)argument.Value.Value!);
                    break;
            }

        return (success, [..errors]);
    }

    private EndpointAttributeModel(Route route, string httpMethod, int successCode, ImmutableArray<int> errorCodes) =>
        (Route, HttpMethod, SuccessCode, ErrorCodes) = (route, httpMethod, successCode, errorCodes);
}
