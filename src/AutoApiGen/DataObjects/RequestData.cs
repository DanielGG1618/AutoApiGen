using System.Collections.Immutable;

namespace AutoApiGen.DataObjects;

internal readonly record struct RequestData(
    string Name,
    IImmutableList<ParameterData> Parameters    
);
