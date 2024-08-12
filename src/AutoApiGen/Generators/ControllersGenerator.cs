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
        
        var templatesProviders = new EmbeddedResourceTemplatesProvider();
        var controllers = new Dictionary<string, ControllerData>();
        
        var rootNamespace = compilation.AssemblyName;
        var controllersNamespace = $"{rootNamespace}.Controllers";
        
        foreach (var endpoint in endpoints)
        {
            var actionName = endpoint.GetActionName();
            var contractType = endpoint.GetContractType(); //TODO extract params from here to request data object
            
            var request = new RequestData(
                $"{actionName}Request",
                Parameters: []
            );
            
            var method = new MethodData(
                endpoint.GetHttpMethod(),
                endpoint.GetRelationalRoute(),
                Attributes: [],
                actionName,
                Parameters: [],
                request.Name,
                contractType,
                endpoint.GetResponseType()
            );

            var controllerName = endpoint.GetControllerName();

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

        var template = templatesProviders.GetFor<ControllerData>();
        var templatesRenderer = new TemplatesRenderer(new EmbeddedResourceTemplatesProvider());
        foreach (var controller in controllers.Values)
        {
            context.AddSource(
                $"{controller.Name}Controller.g.cs",
                templatesRenderer.RenderTemplate(template, with: controller)
            );
        }
    }
}
