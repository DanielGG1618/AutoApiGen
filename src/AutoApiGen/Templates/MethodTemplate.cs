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

    public static string Render(
        Data data,
        Func<ParameterTemplate.Data, string> renderParameter,
        Func<ImmutableArray<string>?, string, string> renderDeconstruction
    ) => $$"""
        [global::Microsoft.AspNetCore.Mvc.Http{{data.HttpMethod}}{{data.Route.ApplyIfNotNullOrEmpty(route => $"({route})")}}]
        public async global::System.Threading.Tasks.Task<global::Microsoft.AspNetCore.Mvc.IActionResult> {{data.Name}}(
            {{data.RequestType.ApplyIfNotNullOrEmpty(requestType =>
                $"[global::Microsoft.AspNetCore.Mvc.FromBody] {requestType} request,"
            )}}
            {{data.Parameters.RenderAndJoin(renderParameter, separator: ",\n").ApplyIfNotNullOrEmpty(s => s + ',')}}
            global::System.Threading.CancellationToken cancellationToken = default
        ) 
        {
            {{renderDeconstruction(data.RequestParameterNames, "request")}} 
            
            var contract = new {{data.ContractType}}(
                {{string.Join(separator: ",\n",
                    data.ContractParameterNames.Select(parameterName => $"{parameterName}: {parameterName}"))
                }}
            );
            
            var result = await _mediator.Send(contract, cancellationToken);
            
            return Ok(result);
        }
        """;
}
