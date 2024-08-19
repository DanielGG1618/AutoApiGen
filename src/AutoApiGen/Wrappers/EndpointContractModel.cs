using AutoApiGen.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static AutoApiGen.StaticData;

namespace AutoApiGen.Wrappers;

internal readonly record struct EndpointContractModel
{
    private static string[] Suffixes { get; } =
    [
        "Request",
        "Command",
        "Query",
    ];
    
    public EndpointAttributeModel Attribute { get; }
    public string ContractTypeFullName { get; }
    public string RequestTypeFullName { get; }
    public string ResponseTypeFullName { get; }
    public IReadOnlyList<ParameterSyntax> Parameters { get; }

    public static EndpointContractModel Create(TypeDeclarationSyntax type)
    {
        if (!IsValid(type))
            throw new ArgumentException("Provided type is not valid Endpoint Contract");

        var typeName = type.Name();
        
        return new EndpointContractModel(
            attribute: EndpointAttributeModel.Create(
                type.GetAttributes().Single(attr =>
                    EndpointAttributeNames.Contains(attr.Name.ToString())
                )
            ),
            contractTypeName: type.GetFullName(), //TODO this is not actually fullname
            requestName: type.Parent is TypeDeclarationSyntax parent
                ? parent.Name()
                : Suffixes.SingleOrDefault(suffix => typeName.EndsWith(suffix)) is string matchingSuffix
                    ? typeName.Remove(typeName.Length - matchingSuffix.Length)
                    : typeName,
            type.GetGenericTypeParametersOfInterface("IRequest").SingleOrDefault()
            ?? (
                type.GetGenericTypeParametersOfInterface("ICommand").SingleOrDefault()
                ?? (
                    type.GetGenericTypeParametersOfInterface("IQuery").SingleOrDefault()
                    ?? throw new InvalidOperationException("Response type is not specified")
                )
            ),
            type.GetConstructorParameters()
        );
    }

    public static bool IsValid(TypeDeclarationSyntax type) =>
        type.BaseList?.Types.Any(baseType =>
            baseType.Type is SimpleNameSyntax { Identifier.Text: "IRequest" or "ICommand" or "IQuery" }
        ) is true;

    private EndpointContractModel(
        EndpointAttributeModel attribute,
        string contractTypeName,
        string requestName,
        string responseType,
        IReadOnlyList<ParameterSyntax> parameters
    ) => (Attribute, ContractTypeFullName, RequestTypeFullName, ResponseTypeFullName, Parameters) =
        (attribute, contractTypeName, requestName, responseType, parameters);
}
