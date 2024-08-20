using AutoApiGen.TemplatesProcessing;
using Microsoft.CodeAnalysis;

namespace AutoApiGen.Generators;

[Generator]
internal class StaticCodeGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(AddEndpointAttributes);
    }

    private static void AddEndpointAttributes(IncrementalGeneratorPostInitializationContext context) =>
        context.AddSource("EndpointAttributes.g.cs",
            EmbeddedResource.GetContent("Templates.EndpointAttributes.txt")
        );
}
