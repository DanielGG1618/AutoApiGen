using AutoApiGen.Templates;
using Scriban;

namespace AutoApiGen.TemplatesProcessing;

internal interface ITemplatesProvider
{
    Template GetFor<T>() where T : ITemplateData;
}
