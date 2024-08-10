using System.Text.RegularExpressions;

namespace AutoApiGen;

public class Regexes
{
    public static readonly Regex RawParameterRoutePartRegex = new(
        """
        ^{
            (?<name>[a-zA-Z_]\w*)
            (?::(?<type>[a-zA-Z_]\w*))?
            (?:=(?<default>.+))?
        }$
        """,
        RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace
    );
    
    public static readonly Regex OptionalParameterRoutePartRegex = new(
        """
        ^{
            (?<name>[a-zA-Z_]\w*)
            (?::(?<type>[a-zA-Z_]\w*))?
            \?
        }$
        """,
        RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace
    );
    
    public static readonly Regex CatchAllParameterRoutePartRegex = new(
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
