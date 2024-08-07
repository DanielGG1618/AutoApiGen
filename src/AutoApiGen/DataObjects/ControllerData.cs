namespace AutoApiGen.DataObjects;

internal record ControllerData(
    string Namespace,
    string BaseRoute,
    string Name,
    IImmutableList<MethodData> Methods
);
