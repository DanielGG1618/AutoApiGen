using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AutoApiGen.Extensions;

internal static class TypeDeclarationSyntaxExtensions
{
    public static string Name(this TypeDeclarationSyntax type) =>
        type.Identifier.Text;

    public static IEnumerable<AttributeSyntax> GetAttributes(this TypeDeclarationSyntax type) =>
        type.AttributeLists.SelectMany(list => list.Attributes);

    public static bool HasAttributeWithNameFrom(
        this TypeDeclarationSyntax type,
        ISet<string> names
    ) => type.GetAttributes().ContainsAttributeWithNameFrom(names);

    public static bool HasAttributeWithNameFrom(
        this TypeDeclarationSyntax type,
        ISet<string> names,
        out AttributeSyntax attribute
    ) => type.GetAttributes().ContainsAttributeWithNameFrom(names, out attribute);

    public static IEnumerable<string> GetGenericTypeParametersOfInterface(
        this TypeDeclarationSyntax type,
        string interfaceName
    ) => type.BaseList?.Types
             .Where(baseType =>
                 baseType.Type is GenericNameSyntax genericName
                 && genericName.Identifier.Text == interfaceName
             )
             .SelectMany(baseType => ((GenericNameSyntax)baseType.Type).TypeArgumentList.Arguments)
             .Select(typeArgument => typeArgument.ToString())
         ?? [];

    public static string GetFullName(this TypeDeclarationSyntax type)
    {
        var pathParts = new List<string>(capacity: 4);

        for (var current = type.Parent; current is not (null or CompilationUnitSyntax); current = current.Parent)
            pathParts.Add(current switch
                {
                    NamespaceDeclarationSyntax @namespace => @namespace.Name.ToString(),
                    FileScopedNamespaceDeclarationSyntax @namespace => @namespace.Name.ToString(),
                    TypeDeclarationSyntax parentType => parentType.Name(),
                    _ => throw new ArgumentOutOfRangeException()
                }
            );

        pathParts.Reverse();
        pathParts.Add(type.Name());

        return "global::" + string.Join(separator: ".", pathParts);
    }

    public static string GetNamespace(this TypeDeclarationSyntax type)
    {
        var namespaces = new List<string>();

        for (var current = type.Parent; current is not (null or CompilationUnitSyntax); current = current.Parent)
            switch (current)
            {
                case NamespaceDeclarationSyntax @namespace:
                    namespaces.Add(@namespace.Name.ToString());
                    break;
                case FileScopedNamespaceDeclarationSyntax @namespace:
                    namespaces.Add(@namespace.Name.ToString());
                    break;
            }

        namespaces.Reverse();
        return "global::" + string.Join(".", namespaces);
    }

    public static IReadOnlyList<ParameterSyntax> GetConstructorParameters(this TypeDeclarationSyntax type) =>
        type switch
        {
            RecordDeclarationSyntax record => record.ParameterList?.Parameters,
            _ => type.Members.OfType<ConstructorDeclarationSyntax>().FirstOrDefault()?.ParameterList.Parameters
        }
        ?? [];
}
