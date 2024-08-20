using System.CodeDom.Compiler;

namespace AutoApiGen.Templates;

internal static class ControllerTemplate
{
    internal readonly record struct Data(
        string MediatorPackageName,
        string Namespace,
        string? BaseRoute,
        string Name,
        List<MethodTemplate.Data> Methods,
        List<RequestTemplate.Data> Requests
    ) : ITemplateData;

    public static void RenderTo(
        IndentedTextWriter indentedWriter,
        Data data,
        Func<RequestTemplate.Data, string> renderRequest,
        Action<IndentedTextWriter, MethodTemplate.Data> renderMethodTo
    )
    {
        indentedWriter.WriteLine(StaticData.GeneratedDisclaimer);
        indentedWriter.WriteLine();
        indentedWriter.WriteLine($"namespace {data.Namespace};");

        if (data.Requests.Count > 0)
        {
            indentedWriter.WriteLine();
            indentedWriter.WriteLine(data.Requests.RenderAndJoin(renderRequest, separator: "\n\n"));
        }

        indentedWriter.WriteLine();
        indentedWriter.WriteLine($"[global::Microsoft.AspNetCore.Mvc.Route(\"{data.BaseRoute}\")]");

        indentedWriter.WriteLines($$"""
            [global::Microsoft.AspNetCore.Mvc.ApiController]
            public partial class {{data.Name}}Controller(
                {{data.MediatorPackageName}}.IMediator mediator
            ) : global::Microsoft.AspNetCore.Mvc.ControllerBase
            {   
                private readonly {{data.MediatorPackageName}}.IMediator _mediator = mediator;
                
            """
        );

        indentedWriter.Indent++;
        foreach (var method in data.Methods)
        {
            renderMethodTo(indentedWriter, method);
            indentedWriter.WriteLine();
        }

        indentedWriter.Indent--;
        indentedWriter.WriteLine('}');
    }
}

file static class ControllerIndentedTextWriterExtensions
{
    public static void WriteLines(this IndentedTextWriter writer, string text)
    {
        foreach (var line in text.Split('\n'))
        {
            writer.WriteLine(line);
        }
    }
}