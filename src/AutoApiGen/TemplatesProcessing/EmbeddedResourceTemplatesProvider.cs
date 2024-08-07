using Scriban;

namespace AutoApiGen.TemplatesProcessing;

internal class EmbeddedResourceTemplatesProvider : ITemplatesProvider
{
    public Template Get() =>
        Template.Parse(EmbeddedResource.GetContent("Templates.Controller.txt"));
}