using System.Diagnostics;
using CodeAnalysis.Diagnostics;
using CodeAnalysis.Evaluation.Values;
using Spectre.Console;

namespace Repl;
internal static class RenderExtensions
{
    private static void Indent(IAnsiConsole console, int indent, string indentString = "  ")
    {
        for (var i = 0; i < indent; ++i)
            console.Write(indentString);
    }

    public static void Write(this IAnsiConsole console, PrimValue value, int indent = 0)
    {
        Indent(console, indent);
        switch (value)
        {
            case FunctionValue function:
                console.Write(new Markup(function.Type.ToString(), "grey66"));
                console.Write(" ");
                console.Write(new Markup(value.Type.Name, "green i"));
                break;
            case LiteralValue literal:
                console.Write(new Markup(literal.Value.ToString()!, "grey66"));
                console.Write(" ");
                console.Write(new Markup(value.Type.Name, "green i"));
                break;
            case ObjectValue @object:
                console.MarkupLine("[grey66]{[/]");
                ++indent;
                foreach (var (ps, pv) in @object)
                {
                    Indent(console, indent);
                    console.MarkupInterpolated($"[grey66]{ps.Name}: [/]");
                    console.WriteLine(pv);
                }
                --indent;
                console.Markup("[grey66]}[/] ");
                console.Write(new Markup(value.Type.Name, "green i"));
                break;
            case StructValue @struct:
                console.Write(new Markup(@struct.Type.ToString(), "grey66"));
                console.Write(" ");
                console.Write(new Markup(value.Type.Name, "green i"));
                break;
            default:
                throw new UnreachableException($"Unexpected value '{value.GetType().Name}'");
        }
    }

    public static void WriteLine(this IAnsiConsole console, PrimValue value)
    {
        console.Write(value);
        console.WriteLine();
    }

    public static void Write(this IAnsiConsole console, Diagnostic diagnostic)
    {
        var fileName = diagnostic.Location.FileName;
        var startLine = diagnostic.Location.StartLine + 1;
        var startCharacter = diagnostic.Location.StartCharacter + 1;
        var endLine = diagnostic.Location.EndLine + 1;
        var endCharacter = diagnostic.Location.EndCharacter + 1;

        var span = diagnostic.Location.Range;
        var lineIndex = diagnostic.Location.SourceText.GetLineIndex(span.Start);
        var line = diagnostic.Location.SourceText.Lines[lineIndex];

        var colour = diagnostic.Severity switch
        {
            DiagnosticSeverity.Error => "red",
            DiagnosticSeverity.Warning => "gold3",
            DiagnosticSeverity.Information => "darkslategray3",
            _ => throw new UnreachableException($"Unexpected {nameof(DiagnosticSeverity)} '{diagnostic.Severity}'"),
        };

        var prefixSpan = new Range(line.Range.Start, span.Start);
        var suffixSpan = new Range(span.End, line.Range.End);

        var prefix = diagnostic.Location.SourceText[prefixSpan];
        var highlight = diagnostic.Location.SourceText[span];
        var suffix = diagnostic.Location.SourceText[suffixSpan];

        var underline = string.Empty;
        if (startLine == endLine)
        {
            underline = String.Create(diagnostic.Location.StartCharacter + highlight.Length, diagnostic.Location.StartCharacter, static (span, start) =>
            {
                span[..start].Fill(' ');
                span[start..].Fill('^');
            });
        }

        console.Write(new Markup($"""
                [{colour}]{fileName}({startLine},{startCharacter},{endLine},{endCharacter}): {diagnostic.Message}[/]
                    {prefix.ToString()}[{colour}]{highlight.ToString()}[/]{suffix.ToString()}
                    {underline}
                """));
    }

    public static void WriteLine(this IAnsiConsole console, Diagnostic diagnostic)
    {
        console.Write(diagnostic);
        console.WriteLine();
    }
}
