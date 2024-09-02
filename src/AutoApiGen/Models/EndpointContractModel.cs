using System.Collections.Immutable;
using AutoApiGen.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AutoApiGen.Models;

internal readonly record struct EndpointContractModel
{
    private static ImmutableArray<string> InterfaceNames { get; } = ["IRequest", "ICommand", "IQuery"];
    private static ImmutableArray<string> Suffixes { get; } = ["Request", "Command", "Query"];

    public EndpointAttributeModel Attribute { get; }
    public string ContractTypeFullName { get; }
    public string RequestName { get; }
    public ImmutableArray<ParameterModel> Parameters { get; } 
    public TypeModel? ResponseType { get; }

    public static EndpointContractModel Create(INamedTypeSymbol type) =>
        !IsValid(type) ? throw new ArgumentException("Provided type is not valid Endpoint Contract")
            : new EndpointContractModel(
                EndpointAttributeModel.Create(type.GetAttributes().Single(EndpointAttributeModel.IsValid)),
                contractTypeFullName: type.ToString(),
                GetRequestName(type),
                parameters: type.InstanceConstructors
                                .FirstOrDefault(c => c.DeclaredAccessibility is Accessibility.Public)
                                ?.Parameters.Select(ParameterModel.FromSymbol)
                                .ToImmutableArray()
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

    public bool Equals(EndpointContractModel other) =>
        Attribute.Equals(other.Attribute)
        && ContractTypeFullName == other.ContractTypeFullName
        && RequestName == other.RequestName
        && Parameters.EqualsSequentially(other.Parameters)
        && Nullable.Equals(ResponseType, other.ResponseType);

    public override int GetHashCode()
    {
        unchecked
        {
            int hashCode = Attribute.GetHashCode();
            hashCode = (hashCode * 397) ^ ContractTypeFullName.GetHashCode();
            hashCode = (hashCode * 397) ^ RequestName.GetHashCode();
            hashCode = (hashCode * 397) ^ Parameters.GetHashCode();
            hashCode = (hashCode * 397) ^ ResponseType.GetHashCode();
            return hashCode;
        }
    }

    private EndpointContractModel(
        EndpointAttributeModel attribute,
        string contractTypeFullName,
        string requestName,
        ImmutableArray<ParameterModel> parameters,
        TypeModel? responseType
    ) => (Attribute, ContractTypeFullName, RequestName, Parameters, ResponseType) =
        (attribute, contractTypeFullName, requestName, parameters, responseType);
}
