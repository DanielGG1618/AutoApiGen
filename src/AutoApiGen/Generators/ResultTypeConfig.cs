namespace AutoApiGen.Generators;

internal readonly record struct ResultTypeConfig(
    string TypeName,
    string MatchMethodName,
    (string Name, string Implementation)? ErrorHandlerMethod
);
