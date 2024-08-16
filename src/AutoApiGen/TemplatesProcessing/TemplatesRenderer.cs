using System.Collections.Immutable;
using AutoApiGen.DataObjects;
using Scriban;
using Scriban.Runtime;

namespace AutoApiGen.TemplatesProcessing;

internal class TemplatesRenderer(ITemplatesProvider templatesProvider)
{
    private readonly ITemplatesProvider _templatesProvider = templatesProvider;

    public string Render<T>(T templateData) where T : ITemplateData =>
        _templatesProvider.GetFor<T>().Render(CreateContextFor(templateData));

    // Future optimizations:
    //  - cache the context for each type
    //  - cache the template for each type
    //  - import only necessary functions
    private TemplateContext CreateContextFor(ITemplateData data)
    {
        var scriptObject = new ScriptObject();
        scriptObject.Import(data);
        //scriptObject.Import("render_controller", Render<ControllerData>);
        scriptObject.Import("render_request", Render<RequestData>);
        scriptObject.Import("render_method", Render<MethodData>);
        scriptObject.Import("render_parameter", Render<ParameterData>);

        scriptObject.Import("render_deconstruction", RenderDeconstruction);

        return new TemplateContext(scriptObject);
    }

    private static string RenderDeconstruction(IImmutableList<string>? names, string source) => names switch
    {
        null or [] => "",
        [var single] => $"var {single} = {source}.{single};",
        _ => $"({string.Join(", ", names)}) = {source};"
    };
}
