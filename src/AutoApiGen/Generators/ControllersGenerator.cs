using System.Collections.Immutable;
using AutoApiGen.Models;
using AutoApiGen.TemplatesProcessing;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace AutoApiGen.Generators;

[Generator]
internal class ControllersGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var mediatorPackageNameProvider = context.SyntaxProvider.CreateMediatorPackageNameProvider();
        var endpointsProvider = context.SyntaxProvider.CreateEndpointsProvider().Collect();

        var compilationDetails = mediatorPackageNameProvider
            .Combine(context.CompilationProvider)
            .Combine(endpointsProvider);

        context.RegisterSourceOutput(compilationDetails, Execute);
    }

    private static void Execute(
        SourceProductionContext context,
        ((string? MediatorPackageName, Compilation), ImmutableArray<EndpointContractModel>) compilationDetails
    )
    {
        var ((mediatorPackageName, compilation), endpoints) = compilationDetails;
        mediatorPackageName ??= StaticData.DefaultMediatorPackageName;

        var rootNamespace = compilation.AssemblyName;

        var templatesProvider = new EmbeddedResourceTemplatesProvider();
        var templatesRenderer = new TemplatesRenderer(templatesProvider);

        var controllers = new ControllerDataBuilder(endpoints, rootNamespace, mediatorPackageName).Build();

        foreach (var controller in controllers)
            context.AddSource(
                $"{controller.Name}Controller.g.cs",
                Formatted(templatesRenderer.Render(controller))
            );
    }

    private static string Formatted(string code) =>
        CSharpSyntaxTree
            .ParseText(code)
            .GetRoot()
            .NormalizeWhitespace()
            .SyntaxTree
            .GetText()
            .ToString();
}
