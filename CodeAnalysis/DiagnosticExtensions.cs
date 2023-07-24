using CodeAnalysis.Text;

namespace CodeAnalysis;

public static class DiagnosticExtensions
{
    public static void WriteTo(this Diagnostic diagnostic, TextWriter writer)
    {
        var fileName = diagnostic.Location.FileName;
        var startLine = diagnostic.Location.StartLine + 1;
        var startCharacter = diagnostic.Location.StartCharacter + 1;
        var endLine = diagnostic.Location.EndLine + 1;
        var endCharacter = diagnostic.Location.EndCharacter + 1;

        var span = diagnostic.Location.Span;
        var lineIndex = diagnostic.Location.Text.GetLineIndex(span.Start);
        var line = diagnostic.Location.Text.Lines[lineIndex];

        var diagnosticColor = diagnostic.Severity switch
        {
            DiagnosticSeverity.Error => ConsoleColor.DarkRed,
            DiagnosticSeverity.Warning => ConsoleColor.DarkYellow,
            _ => throw new InvalidOperationException($"Unexpected diagnostic severity {diagnostic.Severity}")
        };

        writer.WriteLine();

        writer.WriteLineColored($"{fileName}({startLine},{startCharacter},{endLine},{endCharacter}): {diagnostic}", diagnosticColor);

        var prefixSpan = TextSpan.FromBounds(line.Start, span.Start);
        var suffixSpan = TextSpan.FromBounds(span.End, line.End);

        var prefix = diagnostic.Location.Text[prefixSpan];
        var error = diagnostic.Location.Text[span];
        var suffix = diagnostic.Location.Text[suffixSpan];

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

    public static void WriteTo(this IReadOnlyDiagnosticBag diagnostics, TextWriter writer)
    {
        foreach (var diagnostic in diagnostics.OrderBy(d => d.Location))
            diagnostic.WriteTo(writer);
        writer.WriteLine();
    }
}