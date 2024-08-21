using System.Collections.Immutable;
using AutoApiGen.Models;
using AutoApiGen.Templates;
using Microsoft.CodeAnalysis;

namespace AutoApiGen.Generators;

[Generator]
internal sealed class ControllersGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var mediatorPackageNameProvider = context.SyntaxProvider.CreateMediatorPackageNameProvider().Collect();
        var endpointsProvider = context.SyntaxProvider.CreateEndpointsProvider().Collect();

        var compilationDetails = context.CompilationProvider
            .Combine(mediatorPackageNameProvider)
            .Combine(endpointsProvider);

        context.RegisterSourceOutput(compilationDetails, Execute);
    }

    private static void Execute(
        SourceProductionContext context,
        ((Compilation, ImmutableArray<string>), ImmutableArray<EndpointContractModel>) compilationDetails
    )
    {
        var ((compilation, mediatorPackageNameContainer), endpoints) = compilationDetails;

        var rootNamespace = compilation.AssemblyName;
        var mediatorPackageName = mediatorPackageNameContainer is [string single] ? single
            : StaticData.DefaultMediatorPackageName;
        
        var controllers = new ControllerTemplateDataBuilder(endpoints, rootNamespace, mediatorPackageName).Build();

        foreach (var controller in controllers)
            context.AddSource(
                $"{controller.Name}Controller.g.cs",
                TemplatesRenderer.Render(controller)
            );
    }
}
