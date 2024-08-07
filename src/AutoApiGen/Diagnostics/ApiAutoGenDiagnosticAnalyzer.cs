using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

namespace AutoApiGen.Diagnostics;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
internal class ApiAutoGenDiagnosticAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
    [
        DiagnosticDescriptors.LiteralExpressionRequired,
        DiagnosticDescriptors.ForDebug
    ];

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.ReportDiagnostics);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxNodeAction(AnalyzeSyntaxNode, SyntaxKind.RecordDeclaration);
        context.RegisterSyntaxNodeAction(AnalyzeSyntaxNode, SyntaxKind.ClassDeclaration);
    }

    private static void AnalyzeSyntaxNode(SyntaxNodeAnalysisContext context)
    {
        /*if (context.Node is not TypeDeclarationSyntax type)
            return;
        
        var diagnostic = Diagnostic.Create(
            DiagnosticDescriptors.ForDebug,
            type.Identifier.GetLocation(),
            type.BaseList?.Types.Select(baseType => (baseType.Type as SimpleNameSyntax)?.Identifier.Text).FirstOrDefault() ?? "null"
        );

        context.ReportDiagnostic(diagnostic);*/
    }
}