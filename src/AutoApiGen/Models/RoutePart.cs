using System.Diagnostics.Contracts;
using AutoApiGen.Exceptions;

namespace AutoApiGen.Models;

internal abstract record RoutePart
{
    public sealed record LiteralRoutePart(string Value) : RoutePart;

    public abstract record ParameterRoutePart(string Name, string? Type, string? Default) : RoutePart;

    public sealed record RawParameterRoutePart(string Name, string? Type = null, string? Default = null)
        : ParameterRoutePart(Name, Type, Default);

    public sealed record OptionalParameterRoutePart(string Name, string? Type = null)
        : ParameterRoutePart(Name, Type, null);

    public sealed record CatchAllParameterRoutePart(string Name, string? Type = null, string? Default = null)
        : ParameterRoutePart(Name, Type, Default);

    [Pure]
    public static RoutePart Parse(string part) => part switch
    {
        not ['{', .., '}'] => new LiteralRoutePart(part),

        ['{', .., '}'] when Regexes.RawParameterRoutePartRegex.Match(part) is { Success: true } match =>
            new RawParameterRoutePart(
                match.Groups["name"].Value,
                match.Groups["type"].Value is { Length: > 0 } type ? type : null,
                match.Groups["default"].Value is { Length: > 0 } @default ? @default : null
            ),

        ['{', .., '?', '}'] when Regexes.OptionalParameterRoutePartRegex.Match(part) is { Success: true } match =>
            new OptionalParameterRoutePart(
                match.Groups["name"].Value,
                match.Groups["type"].Value is { Length: > 0 } type ? type : null
            ),

        ['{', '*', .., '}'] when Regexes.CatchAllParameterRoutePartRegex.Match(part) is { Success: true } match =>
            new CatchAllParameterRoutePart(
                match.Groups["name"].Value,
                match.Groups["type"].Value is { Length: > 0 } type ? type : null,
                match.Groups["default"].Value is { Length: > 0 } @default ? @default : null
            ),

        _ => throw new ArgumentException("Invalid route part syntax", nameof(part))
    };

    [Pure]
    public static string Format(RoutePart part) => part switch
    {
        LiteralRoutePart(var value) => value,

        RawParameterRoutePart(var name, var type, var @default) =>
            $"{{{name}{FormatType(type)}{FormatDefault(@default)}}}",

        OptionalParameterRoutePart(var name, var type) =>
            $"{{{name}{FormatType(type)}}}",

        CatchAllParameterRoutePart(var name, var type, var @default) =>
            $"{{{name}{FormatType(type)}{FormatDefault(@default)}}}",

        _ => throw new ThisIsUnionException(nameof(RoutePart))
    };

    private static string FormatType(string? type) => type is null ? "" : $":{type}";
    private static string FormatDefault(string? @default) => @default is null ? "" : $"={@default}";
}
