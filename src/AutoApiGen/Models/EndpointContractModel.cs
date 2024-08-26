using AutoApiGen.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AutoApiGen.Models;

internal readonly record struct EndpointContractModel //TODO : IEquatable<EndpointContractModel>
{
    private static ISet<string> InterfaceNames { get; } = new HashSet<string> { "IRequest", "ICommand", "IQuery" };
    private static string[] Suffixes { get; } = ["Request", "Command", "Query"];

    public EndpointAttributeModel Attribute { get; }
    public string ContractTypeFullName { get; }
    public string RequestName { get; }
    public IReadOnlyList<IParameterSymbol> Parameters { get; }
    public TypeModel? ResponseType { get; }

    public static EndpointContractModel Create(INamedTypeSymbol type) =>
        !IsValid(type) ? throw new ArgumentException("Provided type is not valid Endpoint Contract")
            : new EndpointContractModel(
                EndpointAttributeModel.Create(type.GetAttributes().Single(EndpointAttributeModel.IsValid)),
                contractTypeFullName: type.ToString(),
                GetRequestName(type),
                parameters: type.InstanceConstructors
                                .FirstOrDefault(c => c.DeclaredAccessibility is Accessibility.Public)
                                ?.Parameters
                            ?? [],
                responseType: type.GetTypeArgumentsOfInterfaceNamed(InterfaceNames)
                    .FirstOrDefault() is ITypeSymbol responseTypeSymbol
                    ? TypeModel.FromSymbol(responseTypeSymbol) : null
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
        TypeModel? responseType
    ) => (Attribute, ContractTypeFullName, RequestName, Parameters, ResponseType) =
        (attribute, contractTypeFullName, requestName, parameters, responseType);
}
