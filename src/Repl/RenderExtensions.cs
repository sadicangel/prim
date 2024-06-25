using System.Diagnostics;
using CodeAnalysis.Binding;
using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Diagnostics;
using CodeAnalysis.Interpretation.Values;
using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Expressions;
using CodeAnalysis.Syntax.Operators;
using CodeAnalysis.Syntax.Types;
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
            case ArrayValue array:

                console.MarkupInterpolated($"[grey66]{"["}[/]");
                var first = true;
                foreach (var element in array)
                {
                    if (first) first = false;
                    else console.Write(", ");
                    console.Write(element);
                }
                console.MarkupInterpolated($"[grey66]{"]"}[/] ");
                console.MarkupInterpolated($"[green i]{value.Type.Name}[/]");
                break;
            case FunctionValue function:
                console.MarkupInterpolated($"[grey66]{function.Type}[/] [green i]{value.Type.Name}[/]");
                break;
            case LiteralValue literal:
                console.MarkupInterpolated($"[grey66]{literal.Value}[/] [green i]{value.Type.Name}[/]");
                break;
            case ObjectValue @object:
                console.MarkupLineInterpolated($"[grey66]{"{"}[/]");
                ++indent;
                foreach (var (ps, pv) in @object)
                {
                    Indent(console, indent);
                    console.MarkupInterpolated($"[grey66]{ps.Name}: [/]");
                    console.WriteLine(pv);
                }
                --indent;
                Indent(console, indent);
                console.MarkupInterpolated($"[grey66]{"}"}[/] ");
                console.MarkupInterpolated($"[green i]{value.Type.Name}[/]");
                break;
            case StructValue @struct:
                console.MarkupInterpolated($"[grey66]{@struct.Value.Name}[/] [green i]{value.Type.Name}[/]");
                break;
            case ReferenceValue reference:
                console.Write(reference.ReferencedValue, indent);
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
            underline = string.Create(diagnostic.Location.StartCharacter + highlight.Length, diagnostic.Location.StartCharacter, static (span, start) =>
            {
                span[..start].Fill(' ');
                span[start..].Fill('^');
            });
        }

        console.MarkupInterpolated($"""
            [{colour}]{fileName}({startLine},{startCharacter},{endLine},{endCharacter}): {diagnostic.Message}[/]
            {prefix.ToString()}[{colour}]{highlight.ToString()}[/]{suffix.ToString()}
            {underline}
            """);
    }

    public static void WriteLine(this IAnsiConsole console, Diagnostic diagnostic)
    {
        console.Write(diagnostic);
        console.WriteLine();
    }

    public static void Write(this IAnsiConsole console, SyntaxTree syntaxTree)
    {
        var tree = new Tree($"[aqua]{syntaxTree.CompilationUnit.SyntaxKind}[/]")
            .Style("dim white");

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
                            treeNode.AddNode($"[grey66 i]{trivia.SyntaxKind}[/]");
                        if (token.Value is not null)
                            treeNode.AddNode($"[green3]{token.SyntaxKind}[/] {FormatLiteral(token)}");
                        else
                            treeNode.AddNode($"[green3]{token.SyntaxKind}[/] [darkseagreen2 i]{token.Text}[/]");
                        foreach (var trivia in token.TrailingTrivia)
                            treeNode.AddNode($"[grey66 i]{trivia.SyntaxKind}[/]");
                        break;

                    case TypeSyntax type:
                        WriteTo(child, treeNode.AddNode($"[aqua]{child.SyntaxKind}[/] [darkseagreen2 i]{type.Text}[/]"));
                        // $"{base.Name}: {Type.Name}"
                        break;

                    case OperatorSyntax @operator:
                        WriteTo(child, treeNode.AddNode($"[aqua]{child.SyntaxKind}[/]"));
                        break;

                    case ExpressionSyntax expression:
                        WriteTo(child, treeNode.AddNode($"[aqua]{child.SyntaxKind}[/]"));
                        break;

                    default:
                        throw new NotImplementedException(child.GetType().Name);
                }
            }
        }

        static string FormatLiteral(SyntaxToken token)
        {
            return token.SyntaxKind switch
            {

                SyntaxKind.I32LiteralToken or
                SyntaxKind.U32LiteralToken or
                SyntaxKind.I64LiteralToken or
                SyntaxKind.U64LiteralToken or
                SyntaxKind.F32LiteralToken or
                SyntaxKind.F64LiteralToken => $"[gold3]{token.Value}[/]",
                SyntaxKind.StrLiteralToken => $"[darkorange3]\"{token.Value}\"[/]",
                SyntaxKind.TrueKeyword or
                SyntaxKind.FalseKeyword or
                SyntaxKind.NullKeyword => $"[blue3_1]{token.Value}[/]",
                _ => throw new UnreachableException($"Unexpected {nameof(SyntaxKind)} '{token.SyntaxKind}'"),
            };
        }
    }

    public static void WriteLine(this IAnsiConsole console, SyntaxTree syntaxTree)
    {
        console.Write(syntaxTree);
        console.WriteLine();
    }

    public static void Write(this IAnsiConsole console, BoundTree boundTree)
    {
        var tree = new Tree($"[aqua]{boundTree.CompilationUnit.BoundKind}[/]")
            .Style("dim white");

        WriteTo(boundTree.CompilationUnit, tree);

        console.Write(new Panel(tree).Header(nameof(BoundTree)));

        static void WriteTo(BoundNode boundNode, IHasTreeNodes treeNode)
        {
            foreach (var child in boundNode.Children())
            {
                switch (child)
                {
                    case Symbol symbol:
                        WriteTo(child, treeNode.AddNode($"[green3]{symbol.BoundKind}[/] [darkseagreen2 i]{symbol}[/]"));
                        break;

                    case BoundExpression expression:
                        WriteTo(child, treeNode.AddNode($"[aqua]{child.BoundKind}[/] [darkseagreen2 i]{expression.Type}[/]"));
                        break;

                    default:
                        throw new NotImplementedException(child.GetType().Name);
                }
            }
        }
    }

    public static void WriteLine(this IAnsiConsole console, BoundTree boundTree)
    {
        console.Write(boundTree);
        console.WriteLine();
    }
}
