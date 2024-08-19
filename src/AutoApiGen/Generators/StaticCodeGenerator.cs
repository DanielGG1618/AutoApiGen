using System.Collections.Immutable;
using AutoApiGen.Templates;
using AutoApiGen.TemplatesProcessing;
using Microsoft.CodeAnalysis;

namespace AutoApiGen.Generators;

[Generator]
internal class StaticCodeGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var mediatorPackageNameProvider = context.SyntaxProvider.CreateMediatorPackageNameProvider();

        context.RegisterSourceOutput(mediatorPackageNameProvider, Execute);
    }

    private static void Execute(SourceProductionContext context, ImmutableArray<string?> mediatorPackageNameContainer)
    {
        var mediatorPackageName = mediatorPackageNameContainer is [{} singleValue]
            ? singleValue : StaticData.DefaultMediatorPackageName;

        var templatesProvider = new EmbeddedResourceTemplatesProvider();
        var templatesRenderer = new TemplatesRenderer(templatesProvider);

        context.AddSource(
            "ApiController.g.cs",
            templatesRenderer.Render(new ApiControllerBaseData(mediatorPackageName))
        );
 
        context.AddSource(
            "EndpointAttributes.g.cs",
            EmbeddedResource.GetContent("Templates.EndpointAttributes.txt")
        );
    }
}
