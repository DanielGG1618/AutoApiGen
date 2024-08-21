namespace AutoApiGen.Generators;

internal sealed record ResultTypeConfig(
    string TypeName,
    string MatchMethodName,
    (string Name, string Implementation)? ErrorHandlerMethod
);
