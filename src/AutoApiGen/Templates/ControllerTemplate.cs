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
        indentedWriter.WriteControllerBody(data, renderMethodTo);
    }
}

file static class ControllerIndentedTextWriterExtensions
{
    public static void WriteRequestsIfAny(
        this IndentedTextWriter indentedWriter,
        List<RequestTemplate.Data> requests,
        Func<RequestTemplate.Data, string> renderRequest
    ) => indentedWriter.WriteLinesIf(requests.Count > 0,
        requests.RenderAndJoin(renderRequest, separator: "\n\n"),
        ""
    );

    public static void WriteControllerBody(
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
            renderMethodTo(indentedWriter, method);
            indentedWriter.WriteLine();
        }
        indentedWriter.Indent--;
    }
}