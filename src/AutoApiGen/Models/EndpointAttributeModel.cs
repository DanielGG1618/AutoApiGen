using System.Collections.Immutable;
using AutoApiGen.Extensions;
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

        var (successCode, errorCodes) = GetStatusCodes(attribute.NamedArguments);
        
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

    private static (int Success, ImmutableArray<int> Error) GetStatusCodes(
        ImmutableArray<KeyValuePair<string,TypedConstant>> attributeNamedArguments
    )
    {
        var success = 200;
        var errors = new List<int>();

        foreach (var argument in attributeNamedArguments.AsSpan())
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

    public bool Equals(EndpointAttributeModel other) =>
        Route.Equals(other.Route)
        && HttpMethod == other.HttpMethod
        && SuccessCode == other.SuccessCode
        && ErrorCodes.EqualsSequentially(other.ErrorCodes);

    public override int GetHashCode()
    {
        unchecked
        {
            int hashCode = Route.GetHashCode();
            hashCode = (hashCode * 397) ^ HttpMethod.GetHashCode();
            hashCode = (hashCode * 397) ^ SuccessCode;
            hashCode = (hashCode * 397) ^ ErrorCodes.GetHashCode();
            return hashCode;
        }
    }
    
    private EndpointAttributeModel(
        Route route,
        string httpMethod,
        int successCode,
        ImmutableArray<int> errorCodes
    ) => (Route, HttpMethod, SuccessCode, ErrorCodes) =
        (route, httpMethod, successCode, errorCodes);
}
