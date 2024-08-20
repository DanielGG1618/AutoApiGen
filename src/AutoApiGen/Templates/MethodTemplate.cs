using System.CodeDom.Compiler;
using System.Collections.Immutable;
using AutoApiGen.Extensions;

namespace AutoApiGen.Templates;

internal static class MethodTemplate
{
    internal readonly record struct Data(
        string HttpMethod,
        string Route,
        string Name,
        ImmutableArray<ParameterTemplate.Data> Parameters,
        string? RequestType,
        ImmutableArray<string>? RequestParameterNames,
        string ContractType,
        ImmutableArray<string> ContractParameterNames,
        string? ResponseType
    ) : ITemplateData;

    public static void RenderTo(
        IndentedTextWriter indentedWriter,
        Data data,
        Func<ParameterTemplate.Data, string> renderParameter,
        Func<ImmutableArray<string>?, string, string> renderDeconstruction
    )
    {
        indentedWriter.WriteHttpAttribute(data.HttpMethod, data.Route);
        indentedWriter.WriteSignatureDefinition(data.Name, data.RequestType, data.Parameters, renderParameter);
        indentedWriter.WriteBody(data, renderDeconstruction);
    }
}

file static class MethodIndentedTextWriterExtensions
{
    public static void WriteHttpAttribute(this IndentedTextWriter indentedWriter, string httpMethod, string route) =>
        indentedWriter.WriteLine(
            $"[global::Microsoft.AspNetCore.Mvc.Http{httpMethod}{route.ApplyIfNotNullOrEmpty(static route => $"(\"{route}\")")}]"
        );

    public static void WriteSignatureDefinition(
        this IndentedTextWriter indentedWriter,
        string name,
        string? requestType,
        ImmutableArray<ParameterTemplate.Data> parameters,
        Func<ParameterTemplate.Data, string> renderParameter
    )
    {
        indentedWriter.WriteLine(
            $"public async global::System.Threading.Tasks.Task<global::Microsoft.AspNetCore.Mvc.IActionResult> {name}("
        );
        indentedWriter.WriteParameters(requestType, parameters, renderParameter);
        indentedWriter.WriteLine(")");
    }

    public static void WriteParameters(
        this IndentedTextWriter indentedWriter,
        string? requestType,
        ImmutableArray<ParameterTemplate.Data> parameters,
        Func<ParameterTemplate.Data, string> renderParameter
    )
    {
        indentedWriter.Indent++;
        if (requestType is not (null or ""))
            indentedWriter.WriteLine($"[global::Microsoft.AspNetCore.Mvc.FromBody] {requestType} request,");
        if (parameters.Length > 0)
            indentedWriter.WriteLine(parameters.RenderAndJoin(renderParameter, separator: ",\n") + ',');
        indentedWriter.WriteLine("global::System.Threading.CancellationToken cancellationToken = default");
        indentedWriter.Indent--;
    }

    public static void WriteBody(
        this IndentedTextWriter indentedWriter,
        MethodTemplate.Data data,
        Func<ImmutableArray<string>?, string, string> renderDeconstruction
    )
    {
        indentedWriter.WriteLine('{');
        indentedWriter.Indent++;
        if (data.RequestParameterNames?.Length is > 0)
            indentedWriter.WriteLines(renderDeconstruction(data.RequestParameterNames, "request"), "");
        indentedWriter.WriteContractCreation(data);
        indentedWriter.WriteLine();
        indentedWriter.WriteLines(
            data.ResponseType is null or ""
                ? """
                await _mediator.Send(contract, cancellationToken);

                return NoContent();
                """
                : """
                var result = await _mediator.Send(contract, cancellationToken);

                return Ok(result);
                """
        );
        indentedWriter.Indent--;
        indentedWriter.WriteLine('}');
    }

    private static void WriteContractCreation(this IndentedTextWriter indentedWriter, MethodTemplate.Data data)
    {
        if (data.ContractParameterNames.Length > 0)
        {
            indentedWriter.WriteLine($"var contract = new {data.ContractType}(");
            indentedWriter.WriteContractArguments(data);
            indentedWriter.WriteLine(");");
        }
        else indentedWriter.WriteLine($"var contract = new {data.ContractType}();");
    }

    private static void WriteContractArguments(this IndentedTextWriter indentedWriter, MethodTemplate.Data data)
    {
        indentedWriter.Indent++;
        indentedWriter.WriteLines(
            string.Join(
                separator: ",\n",
                data.ContractParameterNames.Select(static parameterName =>
                    $"{parameterName}: {parameterName}"
                )
            )
        );
        indentedWriter.Indent--;
    }
}
