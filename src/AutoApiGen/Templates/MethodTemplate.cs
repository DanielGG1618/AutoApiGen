using System.CodeDom.Compiler;
using System.Collections.Immutable;
using AutoApiGen.Exceptions;
using AutoApiGen.Extensions;

namespace AutoApiGen.Templates;

internal static class MethodTemplate
{
    internal readonly record struct Data(
        string HttpMethod,
        string Route,
        string Attributes,
        string Name,
        ImmutableArray<ParameterTemplate.Data> Parameters,
        string? RequestType,
        ImmutableArray<string>? RequestParameterNames,
        string ContractType,
        ImmutableArray<string> ContractParameterNames,
        ResponseKind ResponseKind
    );

    public static void RenderTo(
        this Data data,
        IndentedTextWriter writer,
        Func<ParameterTemplate.Data, string> renderParameter,
        Func<ImmutableArray<string>?, string, string> renderDeconstruction
    )
    {
        writer.WriteHttpAttribute(data.HttpMethod, data.Route);
        writer.WriteOtherAttributes(data.Attributes);
        writer.WriteSignatureDefinition(data.Name, data.RequestType, data.Parameters, renderParameter);
        writer.WriteBody(data, renderDeconstruction);
    }
}

file static class MethodIndentedTextWriterExtensions
{
    public static void WriteHttpAttribute(this IndentedTextWriter writer, string httpMethod, string route) =>
        writer.WriteLine(
            $"[global::Microsoft.AspNetCore.Mvc.Http{httpMethod}{route.ApplyIfNotNullOrEmpty(static route => $"(\"{route}\")")}]"
        );

    public static void WriteOtherAttributes(this IndentedTextWriter writer, string attributes) =>
        writer.WriteLines(attributes);

    public static void WriteSignatureDefinition(
        this IndentedTextWriter writer,
        string name,
        string? requestType,
        ImmutableArray<ParameterTemplate.Data> parameters,
        Func<ParameterTemplate.Data, string> renderParameter
    )
    {
        writer.WriteLine(
            $"public async global::System.Threading.Tasks.Task<global::Microsoft.AspNetCore.Mvc.IActionResult> {name}("
        );
        writer.WriteParameters(requestType, parameters, renderParameter);
        writer.WriteLine(")");
    }

    private static void WriteParameters(
        this IndentedTextWriter writer,
        string? requestType,
        ImmutableArray<ParameterTemplate.Data> parameters,
        Func<ParameterTemplate.Data, string> renderParameter
    )
    {
        writer.Indent++;
        if (requestType is not (null or ""))
            writer.WriteLine($"[global::Microsoft.AspNetCore.Mvc.FromBody] {requestType} request,");
        if (parameters.Length > 0)
            writer.WriteLine(parameters.RenderAndJoin(renderParameter, separator: ",\n") + ',');
        writer.WriteLine("global::System.Threading.CancellationToken cancellationToken = default");
        writer.Indent--;
    }

    public static void WriteBody(
        this IndentedTextWriter writer,
        MethodTemplate.Data data,
        Func<ImmutableArray<string>?, string, string> renderDeconstruction
    )
    {
        writer.WriteLine('{');
        writer.Indent++;
        if (data.RequestParameterNames?.Length is > 0)
            writer.WriteLines(
                renderDeconstruction(
                    [..data.RequestParameterNames.Value.Select(StringExtensions.WithLowerFirstLetter)],
                    "request"
                ),
                ""
            );
        writer.WriteContractCreation(data);
        writer.WriteLine();
        writer.WriteRequestSendingAndResultReturing(data.ResponseKind);
        writer.Indent--;
        writer.WriteLine('}');
    }

    private static void WriteContractCreation(this IndentedTextWriter writer, MethodTemplate.Data data)
    {
        if (data.ContractParameterNames.Length > 0)
        {
            writer.WriteLine($"var contract = new {data.ContractType}(");
            writer.WriteContractArguments(data);
            writer.WriteLine(");");
        }
        else writer.WriteLine($"var contract = new {data.ContractType}();");
    }

    private static void WriteContractArguments(this IndentedTextWriter writer, MethodTemplate.Data data)
    {
        writer.Indent++;
        writer.WriteLines(
            string.Join(
                separator: ",\n",
                data.ContractParameterNames.Select(static parameterName =>
                    $"{parameterName}: {parameterName.WithLowerFirstLetter()}"
                )
            )
        );
        writer.Indent--;
    }

    private static void WriteRequestSendingAndResultReturing(
        this IndentedTextWriter writer,
        ResponseKind responseKind
    ) => writer.WriteLines(
        responseKind switch
        {
            ResponseKind.Void =>
                """
                await _mediator.Send(contract, cancellationToken);

                return NoContent();
                """,

            ResponseKind.RawNonVoid(var toActionResult) =>
                $"""
                var result = await _mediator.Send(contract, cancellationToken);

                return {toActionResult.Render("result")};
                """,

            ResponseKind.ResultType(var toActionResult, string match, string onError) =>
                $"""
                var result = await _mediator.Send(contract, cancellationToken);

                return result.{match}(
                    x => {toActionResult.Render("x")},
                    errors => {onError}(errors)
                );
                """,

            _ => throw new ThisIsUnionException(nameof(ResponseKind))
        }
    );
}
