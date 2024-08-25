using System.Collections.Immutable;
using AutoApiGen.Extensions;

namespace AutoApiGen.Templates;

internal static class RequestTemplate
{
    internal readonly record struct Data(
        string Name,
        ImmutableArray<ParameterTemplate.Data> Parameters    
    );

    public static string Render(Data data, Func<ParameterTemplate.Data, string> renderParameter) => $"""
        public record {data.Name}Request(
            {data.Parameters.RenderAndJoin(renderParameter, separator: ",\n\t")}
        );
        """;
}
