using AutoApiGen.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AutoApiGen.Models;

internal readonly record struct EndpointContractModel//TODO : IEquatable<EndpointContractModel>
{
    private static ISet<string> InterfaceNames { get; } = new HashSet<string> { "IRequest", "ICommand", "IQuery" };
    private static string[] Suffixes { get; } = ["Request", "Command", "Query"];
    
    public EndpointAttributeModel Attribute { get; }
    public string ContractTypeFullName { get; }
    public string RequestName { get; }
    public IReadOnlyList<IParameterSymbol> Parameters { get; }
    public string? ResponseTypeFullName { get; }

    public static EndpointContractModel Create(INamedTypeSymbol type) =>
        !IsValid(type) ? throw new ArgumentException("Provided type is not valid Endpoint Contract")
            : new EndpointContractModel(
                EndpointAttributeModel.Create(type.GetAttributes().Single(EndpointAttributeModel.IsValid)),
                contractTypeFullName: type.ToString(),
                GetRequestName(type),
                responseTypeFullName: 
                type.GetTypeArgumentsOfInterfaceNamed(InterfaceNames).FirstOrDefault()?.ToString(),
                parameters: type.InstanceConstructors
                                .FirstOrDefault(c => c.DeclaredAccessibility is Accessibility.Public)?.Parameters
                            ?? []
            );

    private static bool IsValid(ITypeSymbol type) =>
        type.Interfaces.Any(i => InterfaceNames.Contains(i.Name));
    
    public static bool IsValid(TypeDeclarationSyntax type) =>
        type.BaseList?.Types.Any(baseType =>
            baseType.Type is SimpleNameSyntax name
            && InterfaceNames.Contains(name.Identifier.Text)
        ) is true;

    private static string GetRequestName(INamedTypeSymbol type) =>
        type.ContainingSymbol is INamedTypeSymbol parent
            ? parent.Name
            : Suffixes.SingleOrDefault(suffix => type.Name.EndsWith(suffix)) is string matchingSuffix
                ? type.Name.Remove(type.Name.Length - matchingSuffix.Length)
                : type.Name;

    private EndpointContractModel(
        EndpointAttributeModel attribute,
        string contractTypeFullName,
        string requestName,
        IReadOnlyList<IParameterSymbol> parameters,
        string? responseTypeFullName
    ) => (Attribute, ContractTypeFullName, RequestName, Parameters, ResponseTypeFullName) =
        (attribute, contractTypeFullName, requestName, parameters, responseTypeFullName);
}
