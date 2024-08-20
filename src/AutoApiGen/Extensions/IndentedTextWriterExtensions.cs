using System.CodeDom.Compiler;

namespace AutoApiGen.Extensions;

internal static class IndentedTextWriterExtensions
{
    public static void WriteLines(this IndentedTextWriter indentedWriter, string lines, int indentation = 0)
    {
        if (indentation is not 0)
            indentedWriter.Indent += indentation;

        foreach (var line in lines.Split('\n'))
            indentedWriter.WriteLine(line);

        if (indentation is not 0)
            indentedWriter.Indent -= indentation;
    }

    public static void WriteLines(this IndentedTextWriter indentedWriter, params string[] lines)
    {
        foreach (var line in lines)
            indentedWriter.WriteLine(line);
    }

    public static void WriteLinesIf(this IndentedTextWriter indentedWriter, bool condition, params string[] lines)
    {
        if (condition)
            indentedWriter.WriteLines(lines);
    }

    public static void WriteLineIf(this IndentedTextWriter indentedWriter, bool condition, string line)
    {
        if (condition)
            indentedWriter.WriteLine(line);
    }
}
