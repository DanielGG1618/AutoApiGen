using System.Text.RegularExpressions;

namespace AutoApiGen;

internal static class Regexes
{
    public static Regex RawParameterRoutePartRegex { get; } = new(
        """
        ^{
            (?<name>[a-zA-Z_]\w*)
            (?::(?<type>[a-zA-Z_]\w*))?
            (?:=(?<default>.+))?
        }$
        """,
        RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace
    );

    public static Regex OptionalParameterRoutePartRegex { get; } = new(
        """
        ^{
            (?<name>[a-zA-Z_]\w*)
            (?::(?<type>[a-zA-Z_]\w*))?
            \?
        }$
        """,
        RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace
    );

    public static Regex CatchAllParameterRoutePartRegex { get; } = new(
        """
        ^{
            \*
            (?<name>[a-zA-Z_]\w*)
            (?::(?<type>[a-zA-Z_]\w*))?
            (?:=(?<default>.+))?
        }$
        """,
        RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace
    );
}
