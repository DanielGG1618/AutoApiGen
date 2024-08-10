using System.Collections.Immutable;

namespace AutoApiGen.DataObjects;

internal readonly record struct ParameterData(
    IImmutableList<string> Attributes,
    string Type,
    string Name
);
