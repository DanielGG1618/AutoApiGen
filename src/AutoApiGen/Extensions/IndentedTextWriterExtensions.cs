using System.CodeDom.Compiler;

namespace AutoApiGen.Extensions;

internal static class IndentedTextWriterExtensions
{
    public static void WriteLines(this IndentedTextWriter writer, string lines, int indentation = 0)
    {
        if (indentation is not 0)
            writer.Indent += indentation;

        foreach (var line in lines.Split('\n'))
            writer.WriteLine(line);

        if (indentation is not 0)
            writer.Indent -= indentation;
    }

    public static void WriteLines(this IndentedTextWriter writer, params string[] lines)
    {
        foreach (var line in lines)
            writer.WriteLine(line);
    }
}
