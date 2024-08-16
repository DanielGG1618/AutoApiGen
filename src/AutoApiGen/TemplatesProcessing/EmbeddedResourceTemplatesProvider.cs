using AutoApiGen.DataObjects;
using Scriban;

namespace AutoApiGen.TemplatesProcessing;

internal class EmbeddedResourceTemplatesProvider : ITemplatesProvider
{
    public Template GetFor<T>() where T : ITemplateData =>
        Template.Parse(
            EmbeddedResource.GetContent(
                $"Templates.{GetTemplateNameFor<T>()}.txt"
            )
        );

    private static string GetTemplateNameFor<T>() where T : ITemplateData =>
        typeof(T).Name.Remove(typeof(T).Name.Length - "Data".Length);
}