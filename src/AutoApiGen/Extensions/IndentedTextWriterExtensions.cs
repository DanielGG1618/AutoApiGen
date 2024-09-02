using System.CodeDom.Compiler;

namespace AutoApiGen.Extensions;

internal static class IndentedTextWriterExtensions
{
    public static void WriteLines(this IndentedTextWriter writer, string lines, int indentation = 0)
    {
        writer.Indent += indentation;

        foreach (var line in lines.Split('\n').AsSpan())
            writer.WriteLine(line);

        writer.Indent -= indentation;
    }
}
