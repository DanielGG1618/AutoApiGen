namespace AutoApiGen.Templates;

internal readonly record struct ApiControllerBaseData(
    string MediatorPackageName
) : ITemplateData;