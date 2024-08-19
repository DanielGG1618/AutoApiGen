using System.Collections.Immutable;
using AutoApiGen.TemplatesProcessing;
using AutoApiGen.Wrappers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace AutoApiGen.Generators;

[Generator]
internal class ControllersGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var mediatorPackageNameProvider = context.SyntaxProvider.CreateMediatorPackageNameProvider();
        var endpointsProvider = context.SyntaxProvider.CreateEndpointsProvider();

        var compilationDetails = context.CompilationProvider
            .Combine(endpointsProvider)
            .Combine(mediatorPackageNameProvider);

        context.RegisterSourceOutput(compilationDetails, Execute);
    }

    private static void Execute(
        SourceProductionContext context,
        ((Compilation, ImmutableArray<EndpointContractModel>), ImmutableArray<string?>) compilationDetails
    )
    {
        var ((compilation, endpoints), mediatorPackageNameContainer) = compilationDetails;
        
        var rootNamespace = compilation.AssemblyName;
        var mediatorPackageName = mediatorPackageNameContainer is [{} singleValue] 
            ? singleValue : StaticData.DefaultMediatorPackageName;

        var templatesProvider = new EmbeddedResourceTemplatesProvider();
        var templatesRenderer = new TemplatesRenderer(templatesProvider);

        var controllers = new ControllerDataBuilder(endpoints, rootNamespace, mediatorPackageName).Build();

        foreach (var controller in controllers)
        {
            context.AddSource(
                $"{controller.Name}Controller.g.cs",
                Formatted(templatesRenderer.Render(controller))
            );
        }
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
