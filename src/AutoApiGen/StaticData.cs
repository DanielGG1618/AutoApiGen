namespace AutoApiGen;

internal static class StaticData
{
    public static ISet<string> EndpointAttributeNames { get; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        "GetEndpointAttribute",
        "PostEndpointAttribute",
        "PutEndpointAttribute",
        "DeleteEndpointAttribute",
        "HeadEndpointAttribute",
        "PatchEndpointAttribute",
        "OptionsEndpointAttribute",
    };
}
