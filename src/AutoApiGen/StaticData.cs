using System.Collections.Immutable;

namespace AutoApiGen;

internal static class StaticData
{
    public static IImmutableSet<string> EndpointAttributeNames { get; } = new HashSet<string>{
        "GetEndpoint",
        "PostEndpoint",
        "PutEndpoint",
        "DeleteEndpoint",
        "HeadEndpoint",
        "PatchEndpoint",
        "OptionsEndpoint"
    }.ToImmutableHashSet();

    public static IImmutableSet<string> EndpointAttributeNamesWithSuffix { get; } =
        EndpointAttributeNames.Select(a => a + "Attribute").ToImmutableHashSet();
}
