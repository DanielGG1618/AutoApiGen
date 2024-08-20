using AutoApiGen.Templates;
using AutoApiGen.TemplatesProcessing;
using Microsoft.CodeAnalysis;

namespace AutoApiGen.Generators;

[Generator]
internal class StaticCodeGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(AddEndpointAttributes);
        
        var mediatorPackageNameProvider = context.SyntaxProvider.CreateMediatorPackageNameProvider();
        context.RegisterSourceOutput(mediatorPackageNameProvider, Execute);
    }

    private static void AddEndpointAttributes(IncrementalGeneratorPostInitializationContext context) =>
        context.AddSource("EndpointAttributes.g.cs",
            EmbeddedResource.GetContent("Templates.EndpointAttributes.txt")
        );

    private static void Execute(SourceProductionContext context, string mediatorPackageName)
    {
        var templatesProvider = new EmbeddedResourceTemplatesProvider();
        var templatesRenderer = new TemplatesRenderer(templatesProvider);

        context.AddSource(
            "ApiController.g.cs",
            templatesRenderer.Render(new ApiControllerBaseData(mediatorPackageName))
        );
    }
}
