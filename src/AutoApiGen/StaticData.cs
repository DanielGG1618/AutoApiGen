namespace AutoApiGen;

internal static class StaticData
{
    public static ISet<string> EndpointAttributeNames { get; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        "GetEndpoint",
        "PostEndpoint",
        "PutEndpoint",
        "DeleteEndpoint",
        "HeadEndpoint",
        "PatchEndpoint",
        "OptionsEndpoint",
    };
    
    public static string[] EndpointContractSuffixes { get; } =
    [
        "Command",
        "Query",
        "Handler"
    ];
}
