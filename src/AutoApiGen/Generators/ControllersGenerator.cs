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
        var errorOrPackageNameProvider = context.SyntaxProvider.CreateErrorOrPackageNameProvider().Collect();
        var endpointsProvider = context.SyntaxProvider.CreateEndpointsProvider().Collect();

        var compilationDetails = context.CompilationProvider
            .Combine(mediatorPackageNameProvider)
            .Combine(errorOrPackageNameProvider)
            .Combine(endpointsProvider)
            .Select((combined, _) => 
                new Configuration(
                    RootNamespace: combined.Left.Left.Left.AssemblyName,
                    MediatorPackageName: combined.Left.Left.Right.SingleOrDefault()
                                         ?? StaticData.DefaultMediatorPackageName,
                    ErrorOrPackageName: combined.Left.Right.SingleOrDefault(),
                    Endpoints: combined.Right
                )
            );

        context.RegisterSourceOutput(compilationDetails, Execute);
    }

    private static void Execute(
        SourceProductionContext context,
        Configuration configuration
    )
    {
        var (rootNamespace, mediatorPackageName, errorOrPackageName, endpoints) = configuration;

        var controllers =
            new ControllerTemplateDataBuilder(
                endpoints,
                rootNamespace,
                mediatorPackageName,
                errorOrPackageName
            ).Build();

        foreach (var controller in controllers)
            context.AddSource(
                $"{controller.Name}Controller.g.cs",
                TemplatesRenderer.Render(controller)
            );
    }

    private record Configuration(
        string? RootNamespace,
        string MediatorPackageName,
        string? ErrorOrPackageName,
        ImmutableArray<EndpointContractModel> Endpoints
    );
}
