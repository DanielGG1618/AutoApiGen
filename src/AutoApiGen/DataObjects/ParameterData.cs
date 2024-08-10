using System.Collections.Immutable;

namespace AutoApiGen.DataObjects;

internal record ParameterData(
    IImmutableList<string> Attributes,
    string Type,
    string Name
);
