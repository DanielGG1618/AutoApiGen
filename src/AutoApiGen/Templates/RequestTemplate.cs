using System.CodeDom.Compiler;
using System.Collections.Immutable;

namespace AutoApiGen.Templates;

internal readonly record struct RequestTemplate(
    string Name,
    ImmutableArray<ParameterTemplate> Parameters    
)
{
    public void RenderTo(IndentedTextWriter writer, Action<IndentedTextWriter, ParameterTemplate> renderParameterTo)
    {
        writer.Write($"public record {Name}Request(");
        if (Parameters.Length > 0)
        {
            writer.WriteLine();
            writer.Indent++;
            var parametersSpan = Parameters.AsSpan();
            for (var i = 0; i < parametersSpan.Length; i++)
            {
                renderParameterTo(writer, parametersSpan[i]);

                if (i != parametersSpan.Length - 1)
                    writer.WriteLine(',');
            }
            writer.WriteLine();
            writer.Indent--;
        }
        writer.WriteLine(");");
    }
}
