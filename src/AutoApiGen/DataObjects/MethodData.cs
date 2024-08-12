using System.Collections.Immutable;

namespace AutoApiGen.DataObjects;

internal readonly record struct MethodData(
    string HttpMethod,
    string Route,
    string Attributes,
    string Name,
    ImmutableArray<ParameterData> Parameters,
    string RequestType,
    ImmutableArray<string> RequestParameterNames,
    string ContractType,
    ImmutableArray<string> ContractParameterNames,
    string ResponseType
) : ITemplateData;
