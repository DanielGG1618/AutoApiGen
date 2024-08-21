using System.CodeDom.Compiler;
using AutoApiGen.Extensions;

namespace AutoApiGen.Templates;

internal static class ControllerTemplate
{
    internal readonly record struct Data(
        string Namespace,
        string? BaseRoute,
        string Name,
        List<MethodTemplate.Data> Methods,
        List<RequestTemplate.Data> Requests,
        string MediatorPackageName
    ) : ITemplateData;

    public static void RenderTo(
        IndentedTextWriter writer,
        Data data,
        string? onErrorMethod,
        Func<RequestTemplate.Data, string> renderRequest,
        Action<IndentedTextWriter, MethodTemplate.Data> renderMethodTo
    )
    {
        writer.WriteLine(StaticData.GeneratedDisclaimer);
        writer.WriteLine();
        writer.WriteLine($"namespace {data.Namespace};");
        writer.WriteLine();
        writer.WriteRequestsIfAny(data.Requests, renderRequest);
        writer.WriteBody(data, onErrorMethod, renderMethodTo);
    }
}

file static class ControllerIndentedTextWriterExtensions
{
    public static void WriteRequestsIfAny(
        this IndentedTextWriter writer,
        List<RequestTemplate.Data> requests,
        Func<RequestTemplate.Data, string> renderRequest
    )
    {
        if (requests.Count > 0)
            writer.WriteLines(
                requests.RenderAndJoin(renderRequest, separator: "\n\n"),
                ""
            );
    }

    public static void WriteBody(
        this IndentedTextWriter writer,
        ControllerTemplate.Data data,
        string? onErrorMethod,
        Action<IndentedTextWriter, MethodTemplate.Data> renderMethodTo
    )
    {
        writer.WriteLines($$"""
            [global::Microsoft.AspNetCore.Mvc.Route("{{data.BaseRoute}}")]
            [global::Microsoft.AspNetCore.Mvc.ApiController]
            public partial class {{data.Name}}Controller(
                {{data.MediatorPackageName}}.IMediator mediator
            ) : global::Microsoft.AspNetCore.Mvc.ControllerBase
            {   
                private readonly {{data.MediatorPackageName}}.IMediator _mediator = mediator;
                
            """
        );
        if (onErrorMethod is not null)
        {
            writer.Indent++;
            writer.WriteLines(onErrorMethod);
            writer.Indent--;
        }
        writer.WriteMethods(renderMethodTo, data.Methods);
        writer.WriteLine('}');
    }

    private static void WriteMethods(
        this IndentedTextWriter writer,
        Action<IndentedTextWriter, MethodTemplate.Data> renderMethodTo,
        List<MethodTemplate.Data> methods
    )
    {
        writer.Indent++;
        foreach (var method in methods)
        {
            writer.WriteLine();
            renderMethodTo(writer, method);
        }
        writer.Indent--;
    }
}