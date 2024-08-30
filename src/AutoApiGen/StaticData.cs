﻿using System.Collections.Immutable;

namespace AutoApiGen;

internal static class StaticData
{
    public const string DefaultMediatorPackageName = "global::MediatR";

    public static ImmutableArray<string> EndpointAttributeNames { get; } =
    [
        "GetEndpoint",
        "PostEndpoint",
        "PutEndpoint",
        "DeleteEndpoint",
        "HeadEndpoint",
        "PatchEndpoint",
        "OptionsEndpoint"
    ];

    public static ImmutableArray<string> EndpointAttributeNamesWithSuffix { get; } =
    [
        "GetEndpointAttribute",
        "PostEndpointAttribute",
        "PutEndpointAttribute",
        "DeleteEndpointAttribute",
        "HeadEndpointAttribute",
        "PatchEndpointAttribute",
        "OptionsEndpointAttribute"
    ];

    public const string GeneratedDisclaimer =
        """
        //--------------------------------------------------------------------------------
        // <auto-generated>
        //     This code was generated by a AutoApiGen tool.
        // </auto-generated>
        //--------------------------------------------------------------------------------
        """;
}
