namespace AutoApiGen.DataObjects;

public readonly record struct ApiControllerBaseData(
    string MediatorPackageName
) : ITemplateData;