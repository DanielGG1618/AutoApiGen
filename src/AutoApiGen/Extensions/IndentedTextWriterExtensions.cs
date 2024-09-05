using System.CodeDom.Compiler;
using System.Runtime.CompilerServices;

namespace AutoApiGen.Extensions;

internal static class IndentedTextWriterExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteLines(this IndentedTextWriter writer, string lines, int indentation = 0)
    {
        writer.Indent += indentation;

        foreach (var line in lines.Split('\n').AsSpan())
            writer.WriteLine(line);

        writer.Indent -= indentation;
    }
}
