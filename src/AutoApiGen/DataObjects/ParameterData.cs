namespace AutoApiGen.DataObjects;

internal readonly record struct ParameterData(
    string Attributes,
    string Type,
    string Name,
    string? Default
) : ITemplateData;
