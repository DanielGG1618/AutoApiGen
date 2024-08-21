namespace AutoApiGen.ConfigAttributes;

[AttributeUsage(AttributeTargets.Assembly)]
public sealed class ResultTypeConfigurationAttribute : Attribute
{
    public required string TypeName { get; init; }
    public required string MatchMethodName { get; init; } = "Match";
    public string? ErrorHandlerMethodName { get; init; }
    public string? ErrorHandlerMethodImplementation { get; init; }
}
