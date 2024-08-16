using System.Collections.Immutable;
using AutoApiGen.TemplatesProcessing;
using Microsoft.CodeAnalysis;

namespace AutoApiGen.Generators;

[Generator]
public class StaticCodeGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context) => 
        context.RegisterSourceOutput(context.CompilationProvider, Execute);

    private static void Execute(SourceProductionContext context, Compilation details)
    {
        context.AddSource("ApiController.g.cs", EmbeddedResource.GetContent("Templates.ApiControllerBase.txt"));
        context.AddSource("EndpointAttributes.g.cs", EmbeddedResource.GetContent("Templates.EndpointAttributes.txt"));
    }
}
