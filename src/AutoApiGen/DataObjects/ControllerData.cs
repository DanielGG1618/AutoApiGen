namespace AutoApiGen.DataObjects;

internal readonly record struct ControllerData(
    string MediatorPackageName,
    string Namespace,
    string BaseRoute,
    string Name,
    List<MethodData> Methods,
    List<RequestData> Requests
) : ITemplateData;
