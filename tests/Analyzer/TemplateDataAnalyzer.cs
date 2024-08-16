using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Analyzer;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
// ReSharper disable once UnusedType.Global
internal class TemplateDataAnalyzer : DiagnosticAnalyzer
{
    private static readonly DiagnosticDescriptor Descriptor = new(
        id: "AAGIA0001",
        title: "Type name must end with 'Data'",
        messageFormat: "Type '{0}' implements ITemplateData and has to end with 'Data'",
        category: "Naming",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "All classes implementing ITemplateData has to end with 'Data'."
    );

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        [Descriptor];

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        context.RegisterSymbolAction(AnalyzeSymbol, SymbolKind.NamedType);
    }

    private static void AnalyzeSymbol(SymbolAnalysisContext context)
    {
        if (context.Symbol is not INamedTypeSymbol namedTypeSymbol)
            return;

        if (!namedTypeSymbol.Interfaces.Any(i => i.Name is "ITemplateData"))
            return;
        
        if (namedTypeSymbol.Name.EndsWith("Data"))
            return;
        
        foreach (var location in namedTypeSymbol.Locations)
            context.ReportDiagnostic(
                Diagnostic.Create(
                    Descriptor,
                    location,
                    namedTypeSymbol.Name
                )
            );
    }
}