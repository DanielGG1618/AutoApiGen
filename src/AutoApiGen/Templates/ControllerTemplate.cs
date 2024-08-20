using System.Text;

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

    public static string Render(
        Data data,
        Func<RequestTemplate.Data, string> renderRequest,
        Func<MethodTemplate.Data, string> renderMethod
    )
    {
        var stringBuilder = new StringBuilder(StaticData.GeneratedDisclaimer);
        stringBuilder.AppendLine()
            .AppendLine($"namespace {data.Namespace};");
        
        if (data.Requests.Count > 0)
            stringBuilder.AppendLine()
                .AppendLine(data.Requests.RenderAndJoin(renderRequest, separator: "\n\n"));

        stringBuilder.AppendLine();
        
        if (data.BaseRoute is not (null or ""))
            stringBuilder.AppendLine($"[global::Microsoft.AspNetCore.Mvc.Route(\"{data.BaseRoute}\")]");
        
        stringBuilder.Append($$"""
            public partial class {{data.Name}}Controller(
                {{data.MediatorPackageName}}.IMediator mediator
            ) : global::Microsoft.AspNetCore.Mvc.ControllerBase
            {   
                private readonly {{data.MediatorPackageName}}.IMediator _mediator = mediator;
             
                {{data.Methods.RenderAndJoin(renderMethod, separator: "\n\n")}}
            }
            """);

        return stringBuilder.ToString();
    }
}
