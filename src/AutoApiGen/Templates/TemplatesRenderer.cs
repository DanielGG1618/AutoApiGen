using System.CodeDom.Compiler;
using System.Collections.Immutable;

namespace AutoApiGen.Templates;

internal static class TemplatesRenderer
{
    public static string Render(ControllerTemplate.Data data)
    {
        using var writer = new IndentedTextWriter(new StringWriter());
        ControllerTemplate.RenderTo(writer, data, Render, RenderTo);

        return writer.InnerWriter.ToString();
    }

    private static void RenderTo(IndentedTextWriter writer, MethodTemplate.Data data) => 
        MethodTemplate.RenderTo(writer, data, Render, RenderDeconstruction);
    
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
