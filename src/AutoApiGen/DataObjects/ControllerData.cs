namespace AutoApiGen.DataObjects;

internal readonly record struct ControllerData(
    string Namespace,
    string BaseRoute,
    string Name,
    List<MethodData> Methods
) : ITemplateData;
