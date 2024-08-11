using AutoApiGen.TemplatesProcessing;
using Scriban;
using Scriban.Runtime;

namespace AutoApiGen;

internal static class TemplatesRenderer
{
    public static string Render(Template template, object with) =>
        template.Render(CreateContext(with));

    private static TemplateContext CreateContext(object obj)
    {
        var scriptObject = new ScriptObject();
        scriptObject.Import(obj);

        var scribanFunctions = new ScribanFunctions();

        var context = new TemplateContext();
        context.PushGlobal(scriptObject);
        context.PushGlobal(scribanFunctions);

        return context;
    }
}
