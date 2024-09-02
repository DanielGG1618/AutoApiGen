using System.Collections.Immutable;
using AutoApiGen.Extensions;
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
            ).WithComparer(Configuration.Comparer);

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
        ResultTypeConfig? ResultTypeConfiguration,
        ImmutableArray<EndpointContractModel> Endpoints
    )
    {
        public static IEqualityComparer<Configuration> Comparer { get; } = new ConfigurationComparer();
        
        private class ConfigurationComparer : IEqualityComparer<Configuration>
        {
            public bool Equals(Configuration obj, Configuration other) =>
                obj.RootNamespace == other.RootNamespace
                && obj.MediatorPackageName == other.MediatorPackageName 
                && obj.Endpoints.EqualsSequentially(other.Endpoints)
                && Nullable.Equals(obj.ResultTypeConfiguration, other.ResultTypeConfiguration); 

            public int GetHashCode(Configuration obj)
            {
                unchecked
                {
                    int hashCode = obj.RootNamespace != null ? obj.RootNamespace.GetHashCode() : 0;
                    hashCode = (hashCode * 397) ^ obj.MediatorPackageName.GetHashCode();
                    hashCode = (hashCode * 397) ^ obj.ResultTypeConfiguration.GetHashCode();
                    hashCode = (hashCode * 397) ^ obj.Endpoints.GetHashCode();
                    return hashCode;
                }
            }
        }
    }
}