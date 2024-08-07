using AutoApiGen.TemplatesProcessing;
using Microsoft.CodeAnalysis;

namespace AutoApiGen.Generators;

[Generator]
public class StaticCodeGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var provider = context.SyntaxProvider.CreateSyntaxProvider(
            predicate: static (_, _) => false,
            transform: static (_, _) => string.Empty
        );

        var compilationDetails = context.CompilationProvider.Combine(provider.Collect());

        context.RegisterSourceOutput(compilationDetails, Execute);
    }

    private static void Execute(SourceProductionContext context, (Compilation, ImmutableArray<string>) details)
    {
        context.AddSource("ApiController.g.cs", EmbeddedResource.GetContent("Templates.ApiControllerBase.txt"));
        context.AddSource("EndpointAttributes.g.cs", EmbeddedResource.GetContent("Templates.EndpointAttributes.txt"));
    }
}
