using System.Collections.Immutable;

namespace AutoApiGen.Templates;

internal static class TemplatesRenderer
{
    public static string Render(ControllerTemplate.Data data) =>
        ControllerTemplate.Render(data, Render, Render);
    private static string Render(MethodTemplate.Data data) =>
        MethodTemplate.Render(data, Render, RenderDeconstruction);
    private static string Render(ParameterTemplate.Data data) =>
        ParameterTemplate.Render(data);
    private static string Render(RequestTemplate.Data data) =>
        RequestTemplate.Render(data, Render);
    
    private static string RenderDeconstruction(ImmutableArray<string>? names, string source) => names switch
    {
        null or [] => "",
        [var single] => $"var {single} = {source}.{single};",
        _ => $"var ({string.Join(", ", names)}) = {source};"
    };
}
