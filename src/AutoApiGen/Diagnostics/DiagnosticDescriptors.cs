using Microsoft.CodeAnalysis;

namespace AutoApiGen.Diagnostics;

internal static class DiagnosticDescriptors
{
    public static DiagnosticDescriptor LiteralExpressionRequired { get; } = new(
        id: "AAG0001",
        title: "Literal expression required",
        messageFormat: "This context requires a literal expression",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true
    );

    public static DiagnosticDescriptor ForDebug { get; } =  new(
        id: "AAG9000",
        title: "Created for debug only",
        messageFormat: "Thing you provided is {0}",
        category: "Debug",
        defaultSeverity: DiagnosticSeverity.Info,
        isEnabledByDefault: true
    );
}