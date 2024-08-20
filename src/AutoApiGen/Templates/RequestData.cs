using System.Collections.Immutable;

namespace AutoApiGen.Templates;

internal readonly record struct RequestData(
    string Name,
    ImmutableArray<ParameterData> Parameters    
) : ITemplateData;
