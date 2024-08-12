using AutoApiGen.DataObjects;
using Scriban;
using Scriban.Runtime;

namespace AutoApiGen.TemplatesProcessing;

internal class TemplatesRenderer(ITemplatesProvider templatesProvider) : ScriptObject
{
    private readonly ITemplatesProvider _templatesProvider = templatesProvider;
    
    public string RenderTemplate(Template template, object with) =>
        template.Render(CreateContext(with));

    private string Render<T>(T templateData) where T : ITemplateData => 
        _templatesProvider.GetFor<T>().Render(CreateContext(templateData));

    private TemplateContext CreateContext(object obj)
    {
        var scriptObject = new ScriptObject();
        scriptObject.Import(obj);
        //scriptObject.Import("render_controller", Render<ControllerData>);
        scriptObject.Import("render_method", Render<MethodData>);
        scriptObject.Import("render_parameter", Render<ParameterData>);

        return new TemplateContext(scriptObject);
    }
}
