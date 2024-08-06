using System.Diagnostics;
using CodeAnalysis.Binding;
using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Diagnostics;
using CodeAnalysis.Interpretation.Values;
using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Expressions;
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

    private static void WriteType(this IAnsiConsole console, TypeSymbol type)
    {
        console.MarkupInterpolated($"[green i]{Markup.Escape(type.QualifiedName)}[/]");
    }

    private static void WriteValue(this IAnsiConsole console, object value, int indent = 0)
    {
        switch (value)
        {
            case ArrayValue array:
                {
                    Indent(console, indent);
                    console.MarkupInterpolated($"[grey66]{Markup.Escape("[")}[/]");
                    var first = true;
                    foreach (var element in array)
                    {
                        if (first) first = false;
                        else console.Write(", ");
                        console.WriteValue(element, indent);
                    }
                    console.MarkupInterpolated($"[grey66]{Markup.Escape("]")}[/]");
                }
                break;
            case ErrorValue error:
                {
                    Indent(console, indent);
                    if (error.IsError)
                        console.MarkupInterpolated($"[maroon]err: [i]{Markup.Escape(error.ErrorMessage ?? "")}[/][/]");
                    else
                        console.WriteValue(error.Value);
                }
                break;
            case InstanceValue instance:
                {
                    if (instance.IsLiteral)
                    {
                        console.WriteValue(instance.Value, indent);
                    }
                    else
                    {
                        Indent(console, indent);
                        console.MarkupLineInterpolated($"[grey66]{"{"}[/]");
                        ++indent;
                        foreach (var (ps, pv) in instance)
                        {
                            Indent(console, indent);
                            console.MarkupInterpolated($"[grey66]{Markup.Escape(ps.Name)}: [/]");
                            console.WriteLine(pv);
                        }
                        --indent;
                        Indent(console, indent);
                        console.MarkupInterpolated($"[grey66]{Markup.Escape("}")}[/]");
                    }
                }
                break;
            case LambdaValue lambda:
                {
                    Indent(console, indent);
                    console.MarkupInterpolated($"[grey66]{Markup.Escape(lambda.Type.ToString())}[/]");
                }
                break;
            case OptionValue option:
                {
                    Indent(console, indent);
                    console.Write("[ ");
                    console.WriteValue(option.Value);
                    console.Write(" ] ");
                }
                break;
            case ReferenceValue reference:
                {
                    console.WriteValue(reference.ReferencedValue, indent);
                }
                break;
            case StructValue @struct:
                {
                    Indent(console, indent);
                    console.MarkupInterpolated($"[grey66]{Markup.Escape(@struct.Name)}[/]");
                }
                break;
            case UnionValue union:
                {
                    Indent(console, indent);
                    console.Write("[ ");
                    console.Write(union.Value);
                    console.Write(" ] ");
                }
                break;
            case ModuleValue module:
                {
                    Indent(console, indent);
                    console.Write(module.Name);
                }
                break;
            default:
                console.MarkupInterpolated($"[grey66]{Markup.Escape(value.ToString() ?? "")}[/]");
                break;
        }
    }

    public static void Write(this IAnsiConsole console, PrimValue value, int indent = 0)
    {
        console.WriteValue(value, indent);
        console.Write(" ");
        console.WriteType(value.Type);
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
            [{colour}]{Markup.Escape(fileName)}({startLine},{startCharacter},{endLine},{endCharacter}): {Markup.Escape(diagnostic.Message)}[/]
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
                            treeNode.AddNode($"[green3]{token.SyntaxKind}[/] [darkseagreen2 i]{Markup.Escape(token.Text.ToString())}[/]");
                        foreach (var trivia in token.TrailingTrivia)
                            treeNode.AddNode($"[grey66 i]{trivia.SyntaxKind}[/]");
                        break;

                    case TypeSyntax type:
                        WriteTo(child, treeNode.AddNode($"[aqua]{child.SyntaxKind}[/] [darkseagreen2 i]{Markup.Escape(type.Text.ToString())}[/]"));
                        // $"{base.Name}: {Type.Name}"
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
                >= SyntaxKind.I8LiteralToken and <= SyntaxKind.F64LiteralToken => $"[gold3]{token.Value}[/]",
                SyntaxKind.StrLiteralToken => $"[darkorange3]\"{Markup.Escape(token.Value?.ToString() ?? "")}\"[/]",
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
                        WriteTo(child, treeNode.AddNode($"[gold3]{symbol.BoundKind}[/] [darkseagreen2 i]{Markup.Escape(symbol.ToString())}[/]"));
                        break;

                    //case BoundExpression expression when expression.ConstValue is not null:
                    //    WriteTo(child, treeNode.AddNode($"[aqua]{child.BoundKind}[/] ({expression.ConstValue}) [darkseagreen2 i]{expression.Type}[/]"));
                    //    break;

                    case BoundExpression expression:
                        WriteTo(child, treeNode.AddNode($"[aqua]{child.BoundKind}[/] [darkseagreen2 i]{Markup.Escape(expression.Type.Name)}[/]"));
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

    public static void Write(this IAnsiConsole console, ScopeSymbol scope)
    {
        var tree = ToTree(scope);

        while (scope != scope.ContainingScope)
        {
            scope = scope.ContainingScope;
            var node = ToTree(scope);
            node.AddNode(tree);
            tree = node;
        }

        console.Write(new Panel(tree).Header(scope.Name));

        static Tree ToTree(ScopeSymbol scope)
        {
            var tree = new Tree($"[darkorange3_1]{scope.Name}[/]")
                .Style("dim white");

            foreach (var symbol in scope.DeclaredSymbols)
            {
                if (symbol is ScopeSymbol innerScope)
                {
                    tree.AddNode(ToTree(innerScope));
                }
                else
                {
                    tree.AddNode($"{Markup.Escape(symbol.Name)} [green i]{Markup.Escape(symbol.Type.Name)}[/]");
                }
            }

            return tree;
        }
    }

    public static void WriteLine(this IAnsiConsole console, ScopeSymbol scope)
    {
        console.Write(scope);
        console.WriteLine();
    }
}
