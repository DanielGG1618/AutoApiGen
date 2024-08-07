namespace AutoApiGen.DataObjects;

internal record MethodData(
    string HttpMethod,
    string Route,
    IImmutableList<string> Attributes,
    string Name,
    IImmutableList<ParameterData> Parameters,
    string RequestType,
    string ResponseType
);
