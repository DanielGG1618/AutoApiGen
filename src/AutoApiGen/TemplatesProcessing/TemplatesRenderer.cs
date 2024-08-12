using AutoApiGen.DataObjects;
using Scriban;
using Scriban.Runtime;

namespace AutoApiGen.TemplatesProcessing;

internal class TemplatesRenderer(ITemplatesProvider templatesProvider) : ScriptObject
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
        scriptObject.Import("render_method", Render<MethodData>);
        scriptObject.Import("render_parameter", Render<ParameterData>);

        return new TemplateContext(scriptObject);
    }
}
