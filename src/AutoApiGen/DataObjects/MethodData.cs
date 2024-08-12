using System.Collections.Immutable;

namespace AutoApiGen.DataObjects;

internal readonly record struct MethodData(
    string HttpMethod,
    string Route,
    ImmutableArray<string> Attributes,
    string Name,
    ImmutableArray<ParameterData> Parameters,
    string RequestType,
    string ContractType,
    string ResponseType
) : ITemplateData;
