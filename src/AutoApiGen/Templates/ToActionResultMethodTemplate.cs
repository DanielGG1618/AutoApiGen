using System.Collections.Immutable;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Text;

namespace AutoApiGen.Templates;

internal readonly record struct ToActionResultMethodTemplate(
    string Name,
    ImmutableArray<string> ExternalParameters,
    bool IncludeInternalResult
)
{
    private static ToActionResultMethodTemplate Ok { get; } = new("Ok", [], IncludeInternalResult: true);

    private static ToActionResultMethodTemplate NoContent { get; } = new("NoContent", [], IncludeInternalResult: false);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    private static ToActionResultMethodTemplate StatusCode(int code) => new("StatusCode",
        [code.ToString()],
        IncludeInternalResult: true
    );

    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static ToActionResultMethodTemplate For(int statusCode) => statusCode switch
    {
        200 => Ok,
        204 => NoContent,
        _ => StatusCode(statusCode),
    };

    public string Render(string? internalResultName = null)
    {
        if (IncludeInternalResult && internalResultName is null)
            throw new ArgumentNullException(nameof(internalResultName));

        var builder = new StringBuilder(Name);

        builder.Append('(');
        builder.Append(string.Join(separator: ", ", ExternalParameters));
        if (IncludeInternalResult)
        {
            if (ExternalParameters.Length > 0)
                builder.Append(", ");
            builder.Append(internalResultName);
        }
        builder.Append(')');

        return builder.ToString();
    }
}
