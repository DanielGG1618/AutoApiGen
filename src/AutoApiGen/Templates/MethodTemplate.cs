using System.CodeDom.Compiler;
using System.Collections.Immutable;
using AutoApiGen.Extensions;

namespace AutoApiGen.Templates;

internal readonly record struct MethodTemplate(
    string HttpMethod,
    string Route,
    string Attributes,
    string Name,
    ImmutableArray<ParameterTemplate> Parameters,
    string? RequestType,
    ImmutableArray<string>? RequestParameterNames,
    string ContractType,
    ImmutableArray<string> ContractParameterNames,
    ResponseReturnTemplate ResponseReturnTemplate
)
{
    public void RenderTo(
        IndentedTextWriter writer,
        Action<IndentedTextWriter, ParameterTemplate> renderParameterTo,
        Action<IndentedTextWriter, ImmutableArray<string>?, string> renderDeconstructionTo
    )
    {
        RenderHttpAttributeTo(writer);
        writer.WriteLines(Attributes);
        RenderSignatureDefinitionTo(writer, renderParameterTo);
        RenderBodyTo(writer, renderDeconstructionTo);
    }

    private void RenderHttpAttributeTo(IndentedTextWriter writer) =>
        writer.WriteLine(
            $"[global::Microsoft.AspNetCore.Mvc.Http{HttpMethod}{Route.ApplyIfNotNullOrEmpty(static route => $"(\"{route}\")")}]"
        );

    private void RenderSignatureDefinitionTo(
        IndentedTextWriter writer,
        Action<IndentedTextWriter, ParameterTemplate> renderParameterTo
    )
    {
        writer.WriteLine(
            $"public async global::System.Threading.Tasks.Task<global::Microsoft.AspNetCore.Mvc.IActionResult> {Name}("
        );
        RenderParametersTo(writer, renderParameterTo);
        writer.WriteLine(")");
    }

    private void RenderParametersTo(
        IndentedTextWriter writer,
        Action<IndentedTextWriter, ParameterTemplate> renderParameterTo
    )
    {
        writer.Indent++;
        if (RequestType is not (null or ""))
            writer.WriteLine($"[global::Microsoft.AspNetCore.Mvc.FromBody] {RequestType} request,");
        if (Parameters.Length > 0)
        {
            foreach (var parameter in Parameters.AsSpan())
            {
                renderParameterTo(writer, parameter);
                writer.WriteLine(',');
            }
        }
        writer.WriteLine("global::System.Threading.CancellationToken cancellationToken = default");
        writer.Indent--;
    }

    private void RenderBodyTo(
        IndentedTextWriter writer,
        Action<IndentedTextWriter, ImmutableArray<string>?, string> renderDeconstructionTo
    )
    {
        writer.WriteLine('{');
        writer.Indent++;
        if (RequestParameterNames?.Length is > 0)
        {
            renderDeconstructionTo(writer,
                [..RequestParameterNames!.Value.Select(StringExtensions.WithLowerFirstLetter)],
                "request"
            );
            writer.WriteLine();
        }
        RenderContractCreationTo(writer);
        writer.WriteLine();
        ResponseReturnTemplate.RenderTo(writer);
        writer.Indent--;
        writer.WriteLine('}');
    }
    
    private void RenderContractCreationTo(IndentedTextWriter writer)
    {
        if (ContractParameterNames.Length > 0)
        {
            writer.WriteLine($"var contract = new {ContractType}(");
            RenderContractArgumentsTo(writer);
            writer.WriteLine(");");
        }
        else writer.WriteLine($"var contract = new {ContractType}();");
    }
    
    private void RenderContractArgumentsTo(
        IndentedTextWriter writer
    )
    {
        writer.Indent++;
        writer.WriteLines(
            string.Join(
                separator: ",\n",
                ContractParameterNames.Select(static parameterName =>
                    $"{parameterName}: {parameterName.WithLowerFirstLetter()}"
                )
            )
        );
        writer.Indent--;
    }
}
