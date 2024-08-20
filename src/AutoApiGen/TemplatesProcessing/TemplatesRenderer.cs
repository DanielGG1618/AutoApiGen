using System.Collections.Immutable;
using AutoApiGen.Templates;

namespace AutoApiGen.TemplatesProcessing;

internal static class TemplatesRenderer
{
    public static string Render(ITemplateData templateData) => templateData switch
    {
        ControllerTemplate.Data data => ControllerTemplate.Render(data, d => Render(d), d => Render(d)),
        MethodTemplate.Data data => MethodTemplate.Render(data, d => Render(d), RenderDeconstruction),
        ParameterTemplate.Data data => ParameterTemplate.Render(data),
        RequestTemplate.Data data => RequestTemplate.Render(data, d => Render(d)),
        
        _ => throw new ArgumentException("Unsupported template data type")
    };
    
    private static string RenderDeconstruction(ImmutableArray<string>? names, string source) => names switch
    {
        null or [] => "",
        [var single] => $"var {single} = {source}.{single};",
        _ => $"var ({string.Join(", ", names)}) = {source};"
    };
}
