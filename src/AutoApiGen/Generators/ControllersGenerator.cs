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
        var resultTypeConfigProvider = context.SyntaxProvider.CreateResultTypeConfigProvider().Collect();
        var endpointsProvider = context.SyntaxProvider.CreateEndpointsProvider().Collect();

        var compilationDetails = context.CompilationProvider
            .Combine(mediatorPackageNameProvider)
            .Combine(resultTypeConfigProvider)
            .Combine(endpointsProvider)
            .Select((combined, _) =>
                new Configuration(
                    RootNamespace: combined.Left.Left.Left.AssemblyName,
                    MediatorPackageName: combined.Left.Left.Right.SingleOrDefault()
                                         ?? StaticData.DefaultMediatorPackageName,
                    ResultTypeConfiguration: combined.Left.Right.SingleOrDefault(),
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
        var (rootNamespace, mediatorPackageName, resultTypeConfiguration, endpoints) = configuration;

        var controllers =
            new ControllerTemplateDataBuilder(
                endpoints,
                rootNamespace,
                mediatorPackageName,
                resultTypeConfiguration
            ).Build();

        foreach (var controller in controllers)
            context.AddSource(
                $"{controller.Name}Controller.g.cs",
                TemplatesRenderer.Render(controller, resultTypeConfiguration?.ErrorHandlerMethod?.Implementation)
            );
    }

    private readonly record struct Configuration(
        string? RootNamespace,
        string MediatorPackageName,
        in ResultTypeConfig? ResultTypeConfiguration,
        ImmutableArray<EndpointContractModel> Endpoints
    );
}