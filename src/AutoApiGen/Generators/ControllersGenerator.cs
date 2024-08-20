using System.Collections.Immutable;
using AutoApiGen.Models;
using AutoApiGen.Templates;
using Microsoft.CodeAnalysis;

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
        ((string MediatorPackageName, Compilation), ImmutableArray<EndpointContractModel>) compilationDetails
    )
    {
        var ((mediatorPackageName, compilation), endpoints) = compilationDetails;

        var rootNamespace = compilation.AssemblyName;

        var controllers = new ControllerTemplateDataBuilder(endpoints, rootNamespace, mediatorPackageName).Build();

        foreach (var controller in controllers)
            context.AddSource(
                $"{controller.Name}Controller.g.cs",
                TemplatesRenderer.Render(controller)
            );
    }
}
