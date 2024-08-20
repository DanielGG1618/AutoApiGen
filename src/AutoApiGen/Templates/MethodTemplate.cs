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
        ImmutableArray<string> ContractParameterNames
    ) : ITemplateData;

    public static void RenderTo(
        IndentedTextWriter indentedWriter,
        Data data,
        Func<ParameterTemplate.Data, string> renderParameter,
        Func<ImmutableArray<string>?, string, string> renderDeconstruction
    )
    {
        indentedWriter.WriteLine(
            $"[global::Microsoft.AspNetCore.Mvc.Http{data.HttpMethod}{data.Route.ApplyIfNotNullOrEmpty(static route => $"(\"{route}\")")}]"
        );
        indentedWriter.WriteLine(
            $"public async global::System.Threading.Tasks.Task<global::Microsoft.AspNetCore.Mvc.IActionResult> {data.Name}("
        );
        indentedWriter.Indent++;
        if (data.RequestType is not (null or ""))
            indentedWriter.WriteLine($"[global::Microsoft.AspNetCore.Mvc.FromBody] {data.RequestType} request,");
        if (data.Parameters.Length > 0)
            indentedWriter.WriteLine(data.Parameters.RenderAndJoin(renderParameter, separator: ",\n") + ',');
        indentedWriter.WriteLine("global::System.Threading.CancellationToken cancellationToken = default");
        indentedWriter.Indent--;
        indentedWriter.WriteLine(")");
        indentedWriter.WriteLine("{");
        indentedWriter.Indent++;
        if (data.RequestParameterNames?.Length is > 0)
        {
            indentedWriter.WriteLine(renderDeconstruction(data.RequestParameterNames, "request"));
            indentedWriter.WriteLine();
        }

        if (data.ContractParameterNames.Length > 0)
        {
            indentedWriter.WriteLine($"var contract = new {data.ContractType}(");
            indentedWriter.Indent++;
            indentedWriter.WriteLines(
                string.Join(
                    separator: ",\n",
                    data.ContractParameterNames.Select(static parameterName => $"{parameterName}: {parameterName}")
                )
            );
            indentedWriter.Indent--;
            indentedWriter.WriteLine(");");
        }
        else indentedWriter.WriteLine($"var contract = new {data.ContractType}();");
        
        indentedWriter.WriteLine();
        indentedWriter.WriteLine("var result = await _mediator.Send(contract, cancellationToken);");
        indentedWriter.WriteLine();
        indentedWriter.WriteLine("return Ok(result);");
        indentedWriter.Indent--;
        indentedWriter.WriteLine('}');
    }
}
