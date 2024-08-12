﻿using Microsoft.CodeAnalysis;

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

    public static DiagnosticDescriptor InvalidRoutePartSyntax { get; } = new(
        id: "AAG0002",
        title: "Invalid route part syntax",
        messageFormat: "Route part '{0}' is invalid",
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
    
    public static DiagnosticDescriptor UnusedRouteParameter { get; } = new(
        id: "AAG0018",
        title: "Unused route parameter",
        messageFormat: "Route parameter '{0}' is not represented in the request",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true
    );
}