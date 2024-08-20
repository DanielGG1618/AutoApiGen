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
}
