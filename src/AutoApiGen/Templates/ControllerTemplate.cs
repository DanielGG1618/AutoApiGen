using System.CodeDom.Compiler;
using AutoApiGen.Extensions;

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
        indentedWriter.WriteLine();
        indentedWriter.WriteRequestsIfAny(data.Requests, renderRequest);
        indentedWriter.WriteBody(data, renderMethodTo);
    }
}

file static class ControllerIndentedTextWriterExtensions
{
    public static void WriteRequestsIfAny(
        this IndentedTextWriter indentedWriter,
        List<RequestTemplate.Data> requests,
        Func<RequestTemplate.Data, string> renderRequest
    )
    {
        if (requests.Count > 0)
            indentedWriter.WriteLines(
                requests.RenderAndJoin(renderRequest, separator: "\n\n"),
                ""
            );
    }

    public static void WriteBody(
        this IndentedTextWriter indentedWriter,
        ControllerTemplate.Data data,
        Action<IndentedTextWriter, MethodTemplate.Data> renderMethodTo
    )
    {
        indentedWriter.WriteLines($$"""
            [global::Microsoft.AspNetCore.Mvc.Route("{{data.BaseRoute}}")]
            [global::Microsoft.AspNetCore.Mvc.ApiController]
            public partial class {{data.Name}}Controller(
                {{data.MediatorPackageName}}.IMediator mediator
            ) : global::Microsoft.AspNetCore.Mvc.ControllerBase
            {   
                private readonly {{data.MediatorPackageName}}.IMediator _mediator = mediator;
            """
        );
        indentedWriter.WriteMethods(data, renderMethodTo);
        indentedWriter.WriteLine('}');
    }

    private static void WriteMethods(
        this IndentedTextWriter indentedWriter,
        ControllerTemplate.Data data,
        Action<IndentedTextWriter, MethodTemplate.Data> renderMethodTo
    )
    {
        indentedWriter.Indent++;
        foreach (var method in data.Methods)
        {
            indentedWriter.WriteLine();
            renderMethodTo(indentedWriter, method);
        }
        indentedWriter.Indent--;
    }
}