namespace AutoApiGen.DataObjects;

internal readonly record struct ApiControllerBaseData(
    string MediatorPackageName
) : ITemplateData;