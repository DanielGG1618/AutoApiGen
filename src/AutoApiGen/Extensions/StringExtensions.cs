namespace AutoApiGen.Extensions;

internal static class StringExtensions
{
    public static string WithCapitalFirstLetter(this string str) => str.Length switch
    {
        0 => str,
        1 => str.ToUpperInvariant(),
        _ => char.ToUpperInvariant(str[0]) + str.Substring(1)
    };

    /// <summary>
    /// Returns string without first <paramref name="front"/> 
    /// and last <paramref name="back"/> characters
    /// </summary>
    /// <param name="str">Source string</param>
    /// <param name="front">Number of characters to strip from front</param>
    /// <param name="back">Number of characters to strip from back</param>
    /// <returns></returns>
    public static string Strip(this string str, int front, int back) =>
        str.Remove(str.Length - back).Substring(front);
}