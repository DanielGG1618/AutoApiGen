using System.Collections.Immutable;
using AutoApiGen.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using static AutoApiGen.StaticData;

namespace AutoApiGen.Diagnostics;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
internal class ApiAutoGenDiagnosticAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
    [
        DiagnosticDescriptors.UnusedRouteParameter
    ];

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.ReportDiagnostics);
        context.EnableConcurrentExecution();
        
        context.RegisterSyntaxNodeAction(AnalyzeEndpointContractDeclarations, SyntaxKind.ClassDeclaration);
        context.RegisterSyntaxNodeAction(AnalyzeEndpointContractDeclarations, SyntaxKind.RecordDeclaration);
    }

    private static void AnalyzeEndpointContractDeclarations(SyntaxNodeAnalysisContext context)
    {
        if (context.Node is not TypeDeclarationSyntax type)
            return;

        if (!type.HasAttributeWithNameFrom(EndpointAttributeNames))
            return;
        
        //TODO: Implement the logic to analyze the endpoint contract
        // context.ReportDiagnostic(diagnostic);
    }
}