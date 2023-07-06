using CodeAnalysis.Syntax;
using CodeAnalysis.Text;

namespace CodeAnalysis;

public static class DiagnosticBagExtensions
{
    public static void WriteTo(this IEnumerable<Diagnostic> diagnostics, TextWriter writer, SyntaxTree syntaxTree)
    {
        foreach (var diagnostic in diagnostics.OrderBy(d => d.Span))
        {
            var lineIndex = syntaxTree.Text.GetLineIndex(diagnostic.Span.Start);
            var lineNumber = lineIndex + 1;
            var line = syntaxTree.Text.Lines[lineIndex];
            var columnIndex = diagnostic.Span.Start - line.Start;
            var columnNumber = columnIndex + 1;
            var diagnosticColor = diagnostic.IsError ? ConsoleColor.DarkRed : ConsoleColor.DarkYellow;

            writer.WriteLine();

            writer.WriteLineColored($"({lineNumber}, {columnNumber}): {diagnostic}", diagnosticColor);

            var prefixSpan = TextSpan.FromBounds(line.Start, diagnostic.Span.Start);
            var suffixSpan = TextSpan.FromBounds(diagnostic.Span.End, line.End);

            var prefix = syntaxTree.Text[prefixSpan];
            var error = syntaxTree.Text[diagnostic.Span];
            var suffix = syntaxTree.Text[suffixSpan];

            writer.Write("    ");
            writer.Write(prefix.ToString());
            writer.WriteColored(error.ToString(), diagnosticColor);
            writer.Write(suffix.ToString());
            writer.WriteLine();

            writer.Write(new string(' ', 4 + columnIndex));
            writer.Write(new string('˄', error.Length));
            writer.WriteLine();
        }
        writer.WriteLine();
    }
}