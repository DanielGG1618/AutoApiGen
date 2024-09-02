using System.CodeDom.Compiler;
using AutoApiGen.Extensions;

namespace AutoApiGen.Templates;

internal readonly record struct ControllerTemplate(
    string Namespace,
    string? BaseRoute,
    string Name,
    List<MethodTemplate> Methods,
    List<RequestTemplate> Requests,
    string MediatorPackageName
)
{
    public void RenderTo(
        IndentedTextWriter writer,
        string? onErrorMethod,
        Action<IndentedTextWriter, RequestTemplate> renderRequestTo,
        Action<IndentedTextWriter, MethodTemplate> renderMethodTo
    )
    {
        writer.WriteLine(StaticData.GeneratedDisclaimer);
        writer.WriteLine();
        writer.WriteLine($"namespace {Namespace};");
        writer.WriteLine();
        RenderRequestsTo(writer, renderRequestTo);
        RenderBodyTo(writer, onErrorMethod, renderMethodTo);
    }

    private void RenderRequestsTo(
        IndentedTextWriter writer,
        Action<IndentedTextWriter, RequestTemplate> renderRequestTo
    )
    {
        foreach (var request in Requests)
        {
            renderRequestTo(writer, request);
            writer.WriteLine();
        }
    }

    private void RenderBodyTo(
        IndentedTextWriter writer,
        string? onErrorMethod,
        Action<IndentedTextWriter, MethodTemplate> renderMethodTo
    )
    {
        writer.WriteLines($$"""
            [global::Microsoft.AspNetCore.Mvc.Route("{{BaseRoute}}")]
            [global::Microsoft.AspNetCore.Mvc.ApiController]
            public sealed partial class {{Name}}Controller(
                {{MediatorPackageName}}.IMediator mediator
            ) : global::Microsoft.AspNetCore.Mvc.ControllerBase
            {   
                private readonly {{MediatorPackageName}}.IMediator _mediator = mediator;
                
            """
        );
        if (onErrorMethod is not null)
        {
            writer.Indent++;
            writer.WriteLines(onErrorMethod);
            writer.Indent--;
        }
        WriteMethodsTo(writer, renderMethodTo);
        writer.WriteLine('}');
    }

    private void WriteMethodsTo(
        IndentedTextWriter writer,
        Action<IndentedTextWriter, MethodTemplate> renderMethodTo
    )
    {
        writer.Indent++;
        foreach (var method in Methods)
        {
            writer.WriteLine();
            renderMethodTo(writer, method);
        }
        writer.Indent--;
    }
}