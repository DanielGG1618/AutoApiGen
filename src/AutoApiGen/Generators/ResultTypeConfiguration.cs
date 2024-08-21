namespace AutoApiGen.Generators;

internal sealed record ResultTypeConfiguration(
    string TypeName,
    string MatchMethodName,
    (string Name, string Implementation)? ErrorHandlerMethod
);
