using System.Diagnostics;
using CodeAnalysis.Diagnostics;
using CodeAnalysis.Syntax;
using Spectre.Console;

namespace Repl;

internal static class RenderExtensions
{
    extension(IAnsiConsole console)
    {
        public void Write(Diagnostic diagnostic)
        {
            var fileName = diagnostic.SourceSpan.FileName;
            var startLine = diagnostic.SourceSpan.StartLine + 1;
            var startCharacter = diagnostic.SourceSpan.StartCharacter + 1;
            var endLine = diagnostic.SourceSpan.EndLine + 1;
            var endCharacter = diagnostic.SourceSpan.EndCharacter + 1;

            var span = diagnostic.SourceSpan.Range;
            var lineIndex = diagnostic.SourceSpan.SourceText.GetLineIndex(span.Start);
            var line = diagnostic.SourceSpan.SourceText.Lines[lineIndex];

            var colour = diagnostic.Severity switch
            {
                DiagnosticSeverity.Error => "red",
                DiagnosticSeverity.Warning => "gold3",
                DiagnosticSeverity.Information => "darkslategray3",
                _ => throw new UnreachableException($"Unexpected {nameof(DiagnosticSeverity)} '{diagnostic.Severity}'"),
            };

            var prefixSpan = new Range(line.SourceSpan.Range.Start, span.Start);
            var suffixSpan = new Range(span.End, line.SourceSpan.Range.End);

            var prefix = diagnostic.SourceSpan.SourceText[prefixSpan];
            var highlight = diagnostic.SourceSpan.SourceText[span];
            var suffix = diagnostic.SourceSpan.SourceText[suffixSpan];

            var underline = string.Empty;
            if (startLine == endLine)
            {
                underline = string.Create(
                    diagnostic.SourceSpan.StartCharacter + highlight.Length,
                    diagnostic.SourceSpan.StartCharacter,
                    static (span, start) =>
                    {
                        span[..start].Fill(' ');
                        span[start..].Fill('^');
                    });
            }

            console.MarkupInterpolated(
                $"""
                [{colour}]{Markup.Escape(fileName)}({startLine},{startCharacter},{endLine},{endCharacter}): {Markup.Escape(diagnostic.Message)}[/]
                    {prefix.ToString()}[{colour}]{highlight.ToString()}[/]{suffix.ToString()}
                    {underline}
                """);
        }

        public void WriteLine(Diagnostic diagnostic)
        {
            console.Write(diagnostic);
            console.WriteLine();
        }

        public void Write(SyntaxTree syntaxTree)
        {
            var tree = new Tree($"[aqua]{syntaxTree.CompilationUnit.Kind}[/]").Style("dim white");

            WriteTo(syntaxTree.CompilationUnit, tree);

            console.Write(new Panel(tree).Header(nameof(SyntaxTree)));

            static void WriteTo(SyntaxNode syntaxNode, IHasTreeNodes treeNode)
            {
                foreach (var child in syntaxNode.Children())
                {
                    switch (child)
                    {
                        case SyntaxToken token:
                            foreach (var trivia in token.LeadingTrivia)
                                treeNode.AddNode($"[grey66 i]{trivia.Kind}[/]");
                            if (token.Value is not null)
                                treeNode.AddNode($"[green3]{token.Kind}[/] {FormatLiteral(token)}");
                            else
                                treeNode.AddNode($"[green3]{token.Kind}[/] [darkseagreen2 i]{Markup.Escape(token.SourceSpan.ToString())}[/]");
                            foreach (var trivia in token.TrailingTrivia)
                                treeNode.AddNode($"[grey66 i]{trivia.Kind}[/]");
                            break;

                        case SyntaxNode node:
                            WriteTo(child, treeNode.AddNode($"[aqua]{node.Kind}[/]"));
                            break;

                        default:
                            throw new NotImplementedException(child.GetType().Name);
                    }
                }
            }

            static string FormatLiteral(SyntaxToken token)
            {
                return token.Kind switch
                {
                    >= SyntaxKind.I8LiteralToken and <= SyntaxKind.F64LiteralToken => $"[gold3]{token.Value}[/]",
                    SyntaxKind.StrLiteralToken => $"[darkorange3]\"{Markup.Escape(token.Value?.ToString() ?? string.Empty)}\"[/]",
                    SyntaxKind.TrueKeyword or
                        SyntaxKind.FalseKeyword or
                        SyntaxKind.NullKeyword => $"[blue3_1]{token.Value}[/]",
                    _ => throw new UnreachableException($"Unexpected {nameof(SyntaxKind)} '{token.Kind}'"),
                };
            }
        }

        public void WriteLine(SyntaxTree syntaxTree)
        {
            console.Write(syntaxTree);
            console.WriteLine();
        }
    }
}
