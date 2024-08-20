﻿namespace AutoApiGen.Extensions;

internal static class StringExtensions
{
    public static string WithCapitalFirstLetter(this string str) => str.Length switch
    {
        0 => str,
        1 => str.ToUpperInvariant(),
        _ => char.ToUpperInvariant(str[0]) + str[1..]
    };

    public static string WithLowerFirstLetter(this string str) => str.Length switch
    {
        0 => str,
        1 => str.ToLowerInvariant(),
        _ => char.ToLowerInvariant(str[0]) + str[1..]
    };
}