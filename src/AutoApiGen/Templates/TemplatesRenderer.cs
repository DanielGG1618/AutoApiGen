using System.CodeDom.Compiler;
using System.Collections.Immutable;

namespace AutoApiGen.Templates;

internal static class TemplatesRenderer
{
    public static string Render(ControllerTemplate data, string? onErrorMethodName)
    {
        using var writer = new IndentedTextWriter(new StringWriter());

        data.RenderTo(writer,
            onErrorMethodName,
            renderRequestTo: RenderRequestTo,
            renderMethodTo: RenderMethodTo
        );

        return writer.InnerWriter.ToString();
    }

    private static void RenderRequestTo(IndentedTextWriter writer, RequestTemplate request) =>
        request.RenderTo(writer, RenderParameterTo);

    private static void RenderMethodTo(IndentedTextWriter writer, MethodTemplate methodTemplate) =>
        methodTemplate.RenderTo(writer, RenderParameterTo, RenderDeconstructionTo);

    private static void RenderParameterTo(IndentedTextWriter writer, ParameterTemplate parameterTemplate) =>
        parameterTemplate.RenderTo(writer);

    private static void RenderDeconstructionTo(
        IndentedTextWriter writer,
        ImmutableArray<string>? names,
        string source
    ) => writer.WriteLine(names switch
        {
            null or [] => "",

            [var single] =>

                $"{source}.Deconstruct(out var {single});",
            _ => $"var ({string.Join(", ", names)}) = {source};"
        }
    );
}
