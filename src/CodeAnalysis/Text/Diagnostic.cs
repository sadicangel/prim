using Spectre.Console;
using Spectre.Console.Rendering;
using System.Diagnostics;

namespace CodeAnalysis.Text;

public enum DiagnosticSeverity { Error, Warning, Information }

public sealed record class Diagnostic(DiagnosticSeverity Severity, SourceLocation Location, string Message)
{
    public IRenderable ToRenderable()
    {
        var fileName = Location.FileName;
        var startLine = Location.StartLine + 1;
        var startCharacter = Location.StartCharacter + 1;
        var endLine = Location.EndLine + 1;
        var endCharacter = Location.EndCharacter + 1;

        var span = Location.Range;
        var lineIndex = Location.Source.GetLineIndex(span.Start);
        var line = Location.Source.Lines[lineIndex];

        var colour = Severity switch
        {
            DiagnosticSeverity.Error => "red",
            DiagnosticSeverity.Warning => "gold3",
            DiagnosticSeverity.Information => "darkslategray3",
            _ => throw new UnreachableException($"Unexpected {nameof(DiagnosticSeverity)} '{Severity}'"),
        };

        var prefixSpan = new Range(line.Range.Start, span.Start);
        var suffixSpan = new Range(span.End, line.Range.End);

        var prefix = Location.Source[prefixSpan];
        var highlight = Location.Source[span];
        var suffix = Location.Source[suffixSpan];

        var underline = string.Empty;
        if (startLine == endLine)
        {
            underline = String.Create(Location.StartCharacter + highlight.Length, Location.StartCharacter, static (span, start) =>
            {
                span[..start].Fill(' ');
                span[start..].Fill('^');
            });
        }

        return new Markup($"""
                [{colour}]{fileName}({startLine},{startCharacter},{endLine},{endCharacter}): {Message}[/]
                    {prefix.ToString()}[{colour}]{highlight.ToString()}[/]{suffix.ToString()}
                    {underline}

                """);

        //// Add squiggly line if error span is on the same line.
        //if (startLine == endLine)
        //{
        //    var spacesCount = 4 + Location.StartCharacter;
        //    AnsiConsole.WriteLine(String.Create(spacesCount + highlight.Length, spacesCount, (span, spacesCount) =>
        //    {
        //        span[..spacesCount].Fill(' ');
        //        span[spacesCount..].Fill('˄');
        //    }));
        //}
    }
}