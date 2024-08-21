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
                    ResultTypeConfiguration: new ResultTypeConfiguration(
                        "ErrorOr",
                        "Match",
                        ErrorHandlerMethod: (
                            Name: "Problem",
                            Implementation: """
                            protected global::Microsoft.AspNetCore.Mvc.IActionResult Problem(
                                global::System.Collections.Generic.List<global::ErrorOr.Error> errors
                            )
                            {
                                int statusCode = errors[0].Type switch
                                {
                                    global::ErrorOr.ErrorType.Conflict => 
                                        global::Microsoft.AspNetCore.Http.StatusCodes.Status409Conflict,
                                    global::ErrorOr.ErrorType.Validation => 
                                        global::Microsoft.AspNetCore.Http.StatusCodes.Status400BadRequest,
                                    global::ErrorOr.ErrorType.NotFound => 
                                        global::Microsoft.AspNetCore.Http.StatusCodes.Status404NotFound,
                                    _ => global::Microsoft.AspNetCore.Http.StatusCodes.Status500InternalServerError
                                };
                            
                                return Problem(
                                    statusCode: statusCode,
                                    title: errors[0].Description
                                );
                            }
                            """
                        )
                    ), //combined.Left.Right.SingleOrDefault(),
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

    private sealed record Configuration(
        string? RootNamespace,
        string MediatorPackageName,
        ResultTypeConfiguration? ResultTypeConfiguration,
        ImmutableArray<EndpointContractModel> Endpoints
    );
}