using System.Collections.Immutable;
using AutoApiGen.DataObjects;
using AutoApiGen.Extensions;
using AutoApiGen.TemplatesProcessing;
using AutoApiGen.Wrappers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static AutoApiGen.StaticData;

namespace AutoApiGen.Generators;

[Generator]
internal class ControllersGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var provider = context.SyntaxProvider.CreateSyntaxProvider(
            predicate: static (node, _) =>
                node is TypeDeclarationSyntax { AttributeLists.Count: > 0 } type
                && type.HasAttributeWithNameFrom(EndpointAttributeNames, out var attribute)
                && EndpointAttributeSyntax.IsValid(attribute)
                && EndpointContractDeclarationSyntax.IsValid(type),
            
            transform: static (syntaxContext, _) =>
                EndpointContractDeclarationSyntax.Wrap((TypeDeclarationSyntax)syntaxContext.Node)
        );

        var compilationDetails = context.CompilationProvider.Combine(provider.Collect());

        context.RegisterSourceOutput(compilationDetails, Execute);
    }

    private static void Execute(
        SourceProductionContext context,
        (Compilation, ImmutableArray<EndpointContractDeclarationSyntax>) compilationDetails
    )
    {
        var (compilation, endpoints) = compilationDetails;
        
        var templatesProvider = new EmbeddedResourceTemplatesProvider();
        var templatesRenderer = new TemplatesRenderer(templatesProvider);
        
        var controllers = new Dictionary<string, ControllerData>();
        var requests = new Dictionary<string, RequestData>();
        
        var rootNamespace = compilation.AssemblyName;
        var controllersNamespace = $"{rootNamespace}.Controllers";
        
        foreach (var endpoint in endpoints)
        {
            var requestName = endpoint.GetRequestName();
            var routeParameters = endpoint.GetRouteParameters()
                .Select(parameter =>
                    new ParameterData(
                        Attributes: "[FromRoute]",
                        parameter.Type ?? "string",
                        parameter.Name,
                        parameter.Default
                    )
                ).ToImmutableArray();

            var routeParametersNames = routeParameters.Select(parameter => parameter.Name).ToImmutableHashSet();

            var method = new MethodData(
                endpoint.GetHttpMethod(),
                endpoint.GetRelationalRoute(),
                Attributes: [],
                Name: requestName,
                routeParameters,
                $"{requestName}Request",
                endpoint.GetContractType(),
                endpoint.GetResponseType()
            );
            var requestParameters = endpoint.GetParameters()
                .Where(parameter =>
                    !routeParametersNames.Contains(parameter.Name())
                ).Select(parameter =>
                    new ParameterData(
                        Attributes: "",
                        parameter.Type?.ToFullString() ?? "string",
                        parameter.Name(),
                        parameter.Default?.Value.ToFullString()
                    )
                ).ToImmutableArray();

            var controllerName = endpoint.GetControllerName();
            
            requests[$"{controllerName}.{requestName}"] = new RequestData(
                endpoint.GetNamespace(),
                requestName,
                requestParameters
            );
            
            if (controllers.TryGetValue(controllerName, out var controller))
            {
                controller.Methods.Add(method);
                continue;
            }
            
            controllers[controllerName] = new ControllerData(
                controllersNamespace,
                endpoint.BaseRoute,
                controllerName,
                [method]
            );
        }

        foreach (var controller in controllers.Values)
            context.AddSource(
                $"{controller.Name}Controller.g.cs",
                templatesRenderer.Render(controller)
            );
        
        foreach (var request in requests.Values)
            context.AddSource(
                $"{request.Name}Request.g.cs",
                templatesRenderer.Render(request)
            );
    }
}
