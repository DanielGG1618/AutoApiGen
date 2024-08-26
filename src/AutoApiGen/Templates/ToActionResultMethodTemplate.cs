using System.Collections.Immutable;
using System.Text;

namespace AutoApiGen.Templates;

internal static class ToActionResultMethodTemplate
{
    private static Data Ok { get; } = new("Ok", [], IncludeInternalResult: true);

    private static Data NoContent { get; } = new("NoContent", [], IncludeInternalResult: false);

    private static Data StatusCode(int code) => new("StatusCode",
        [code.ToString()],
        IncludeInternalResult: true
    );

    public static Data For(int statusCode) => statusCode switch
    {
        200 => Ok,
        204 => NoContent,
        _ => StatusCode(statusCode),
    };

    internal readonly record struct Data(
        string Name,
        ImmutableArray<string> ExternalParameters,
        bool IncludeInternalResult
    );

    public static string Render(this Data data, string? internalResultName = null)
    {
        if (data.IncludeInternalResult && internalResultName is null)
            throw new ArgumentNullException(nameof(internalResultName));

        var builder = new StringBuilder(data.Name);

        builder.Append('(');
        builder.Append(string.Join(separator: ", ", data.ExternalParameters));
        if (data.IncludeInternalResult)
        {
            if (data.ExternalParameters.Length > 0)
                builder.Append(", ");
            builder.Append(internalResultName);
        }
        builder.Append(')');

        return builder.ToString();
    }
}
