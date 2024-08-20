using System.Collections.Immutable;
using System.Text;
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

    public static string Render(
        Data data,
        Func<ParameterTemplate.Data, string> renderParameter,
        Func<ImmutableArray<string>?, string, string> renderDeconstruction
    )
    {
        var stringBuilder = new StringBuilder();
        
        stringBuilder.AppendLine(
            $"[global::Microsoft.AspNetCore.Mvc.Http{data.HttpMethod}{data.Route.ApplyIfNotNullOrEmpty(static route => $"(\"{route}\")")}]"
        ).AppendLine(
            $"public async global::System.Threading.Tasks.Task<global::Microsoft.AspNetCore.Mvc.IActionResult> {data.Name}("
        );
        if (data.RequestType is not (null or ""))
            stringBuilder.AppendLine($"[global::Microsoft.AspNetCore.Mvc.FromBody] {data.RequestType} request,");
        if (data.Parameters.Length > 0)
            stringBuilder.AppendLine(data.Parameters.RenderAndJoin(renderParameter, separator: ",\n") + ',');
        stringBuilder.AppendLine("global::System.Threading.CancellationToken cancellationToken = default")
            .AppendLine(")").AppendLine("{");
        if (data.RequestParameterNames?.Length is > 0)
            stringBuilder.AppendLine(renderDeconstruction(data.RequestParameterNames, "request"))
                .AppendLine();
        
        stringBuilder.Append($$"""
                var contract = new {{data.ContractType}}(
                    {{string.Join(separator: ",\n",
                        data.ContractParameterNames.Select(static parameterName => $"{parameterName}: {parameterName}"))
                    }}
                );
                
                var result = await _mediator.Send(contract, cancellationToken);
                
                return Ok(result);
            }
            """
        );

        return stringBuilder.ToString();
    }
}
