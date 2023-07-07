using CodeAnalysis.Syntax;
using CodeAnalysis.Text;

namespace CodeAnalysis;

public static class DiagnosticExtensions
{
    public static void WriteTo(this Diagnostic diagnostic, TextWriter writer, SyntaxTree syntaxTree)
    {
        var fileName = diagnostic.Location.FileName;
        var startLine = diagnostic.Location.StartLine + 1;
        var startCharacter = diagnostic.Location.StartCharacter + 1;
        var endLine = diagnostic.Location.EndLine + 1;
        var endCharacter = diagnostic.Location.EndCharacter + 1;

        var span = diagnostic.Location.Span;
        var lineIndex = syntaxTree.Text.GetLineIndex(span.Start);
        var line = syntaxTree.Text.Lines[lineIndex];

        var diagnosticColor = diagnostic.IsError ? ConsoleColor.DarkRed : ConsoleColor.DarkYellow;

        writer.WriteLine();

        writer.WriteLineColored($"{fileName}({startLine},{startCharacter},{endLine},{endCharacter}): {diagnostic}", diagnosticColor);

        var prefixSpan = TextSpan.FromBounds(line.Start, span.Start);
        var suffixSpan = TextSpan.FromBounds(span.End, line.End);

        var prefix = syntaxTree.Text[prefixSpan];
        var error = syntaxTree.Text[span];
        var suffix = syntaxTree.Text[suffixSpan];

        writer.Write("    ");
        writer.Write(prefix.ToString());
        writer.WriteColored(error.ToString(), diagnosticColor);
        writer.Write(suffix.ToString());
        writer.WriteLine();

        // Add squiggly line if error span is on the same line.
        if (startLine == endLine)
        {
            var spacesCount = 4 + diagnostic.Location.StartCharacter;
            writer.Write(String.Create(spacesCount + error.Length, spacesCount, (span, spacesCount) =>
            {
                span[..spacesCount].Fill(' ');
                span[spacesCount..].Fill('˄');
            }));
            writer.WriteLine();
        }
    }

    public static void WriteTo(this IEnumerable<Diagnostic> diagnostics, TextWriter writer, SyntaxTree syntaxTree)
    {
        foreach (var diagnostic in diagnostics.OrderBy(d => d.Location))
            diagnostic.WriteTo(writer, syntaxTree);
        writer.WriteLine();
    }
}