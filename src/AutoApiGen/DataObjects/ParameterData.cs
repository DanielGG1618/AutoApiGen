using System.Collections.Immutable;

namespace AutoApiGen.DataObjects;

internal readonly record struct ParameterData(
    string Attributes,
    string Type,
    string Name
) : ITemplateData;
