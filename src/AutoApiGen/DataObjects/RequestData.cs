using System.Collections.Immutable;

namespace AutoApiGen.DataObjects;

internal readonly record struct RequestData(
    string Namespace,
    string Name,
    IImmutableList<ParameterData> Parameters    
) : ITemplateData;
