using Scriban;

namespace AutoApiGen.TemplatesProcessing;

internal interface ITemplatesProvider
{
    Template Get();
}
