using System.Collections.Immutable;
using System.Text;
using AutoApiGen.Exceptions;
using AutoApiGen.Extensions;

namespace AutoApiGen.Templates;

internal static class ToActionResultMethodTemplate
{
    private static Data Ok { get; } = new("Ok", [], IncludeInternalResult: true);

    private static Data CreatedAt => new("CreatedAtAction",
        [
            new ParameterData.PostInit("GetActionName"),
            new ParameterData.PostInit("ControllerName"),
            // TODO пизда
            new ParameterData.AnonymousObject([("id", new ParameterData.PropertyAccess("Id"))])
        ],
        IncludeInternalResult: true
    );

    private static Data NoContent { get; } = new("NoContent", [], IncludeInternalResult: false);

    public static Data For(int statusCode) => statusCode switch
    {
        200 => Ok,
        201 => CreatedAt,
        204 => NoContent,

        _ => throw new ArgumentOutOfRangeException(nameof(statusCode), statusCode, "Unknown status code")
    };

    internal abstract record ParameterData
    {
        public sealed record Literal(string Value) : ParameterData;

        public sealed record PostInit(string VariableName) : ParameterData;

        public sealed record PropertyAccess(string PropertyName) : ParameterData;

        public sealed record AnonymousObject(ImmutableArray<(string Name, ParameterData Parameter)> Properties)
            : ParameterData;

        private ParameterData() {}
    }

    internal readonly record struct Data(
        string Name,
        ParameterData[] ExternalParameters,
        bool IncludeInternalResult
    );

    public static string Render(this Data data, string? internalResultName = null)
    {
        if (data.IncludeInternalResult && internalResultName is null)
            throw new ArgumentNullException(nameof(internalResultName));

        var builder = new StringBuilder(data.Name);

        builder.Append('(');
        builder.Append(
            data.ExternalParameters.RenderAndJoin(
                renderer: param => RenderParameterData(param, internalResultName),
                separator: ", "
            )
        );
        if (data.IncludeInternalResult)
        {
            if (data.ExternalParameters.Length > 0)
                builder.Append(", ");
            builder.Append(internalResultName);
        }
        builder.Append(')');

        return builder.ToString();
    }

    private static string RenderParameterData(ParameterData parameter, string? internalResultName) => parameter switch
    {
        ParameterData.Literal literal =>
            $"\"{literal.Value}\"",

        ParameterData.PropertyAccess propAccess => $"{internalResultName}.{propAccess.PropertyName}",

        ParameterData.AnonymousObject anonymous =>
            $"new {{ {
                anonymous.Properties.RenderAndJoin(
                    renderer: param => $"{param.Name} = " + RenderParameterData(param.Parameter, internalResultName),
                    separator: ", "
                )
            } }}",

        ParameterData.PostInit =>
            throw new InvalidOperationException("Parameter uninitialized"),
        //I really do not like previous line of code
        _ => throw new ThisIsUnionException(nameof(ParameterData))
    };
}
