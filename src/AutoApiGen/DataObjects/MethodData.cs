namespace AutoApiGen.DataObjects;

internal readonly record struct MethodData(
    string HttpMethod,
    string Route,
    IImmutableList<string> Attributes,
    string Name,
    IImmutableList<ParameterData> Parameters,
    string RequestType,
    string ResponseType
);
