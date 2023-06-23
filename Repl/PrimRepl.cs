using CodeAnalysis;
using CodeAnalysis.Symbols;
using CodeAnalysis.Syntax;
using CodeAnalysis.Text;

namespace Repl;
internal sealed class PrimRepl : ReplBase
{
    private readonly Dictionary<Symbol, object?> _symbols;
    private bool _showTree;
    private bool _showProgram;
    private bool _showResultType = true;
    private Compilation? _previousCompilation;

    public PrimRepl()
    {
        _symbols = new Dictionary<Symbol, object?>();
        _showTree = false;
        _showProgram = false;
        Console.ForegroundColor = ConsoleColor.White;
    }

    protected override void RenderLine(string line)
    {
        var tokens = SyntaxTree.ParseTokens(line.AsMemory());
        foreach (var token in tokens)
        {
            var color = token.Kind switch
            {
                TokenKind k when k.IsNumber() => ConsoleColor.Cyan,
                TokenKind.String => ConsoleColor.Magenta,
                TokenKind.Identifier => ConsoleColor.DarkYellow,
                TokenKind k when k.IsKeyword() => ConsoleColor.Blue,
                _ => ConsoleColor.DarkGray,
            };

            Console.Out.WriteColored(token.Text, color);
        }
    }

    protected override bool IsCompleteInput(ReadOnlyMemory<char> text)
    {
        if (text.Length == 0)
            return true;

        var syntaxTree = SyntaxTree.Parse(text);

        INode node = syntaxTree.Root.Statement;
        if (node.GetLastToken().IsMissing)
            return false;

        return true;
    }

    protected override bool EvaluateCommand(ReadOnlySpan<char> input)
    {
        switch (input[1..])
        {
            case "help":
                Console.Out.WriteColored("[show help]", ConsoleColor.DarkGray);
                return true;

            case "tree":
                _showTree = !_showTree;
                Console.Out.WriteColored("[show parse tree: ", ConsoleColor.DarkGray);
                Console.Out.WriteColored((_showTree ? "on" : "off"), _showTree ? ConsoleColor.DarkGreen : ConsoleColor.DarkRed);
                Console.Out.WriteLineColored("]", ConsoleColor.DarkGray);
                return true;

            case "program":
                _showProgram = !_showProgram;
                Console.Out.WriteColored("[show bound tree: ", ConsoleColor.DarkGray);
                Console.Out.WriteColored((_showProgram ? "on" : "off"), _showProgram ? ConsoleColor.DarkGreen : ConsoleColor.DarkRed);
                Console.Out.WriteLineColored("]", ConsoleColor.DarkGray);
                return true;

            case "type":
                _showResultType = !_showResultType;
                Console.Out.WriteColored("[show result type: ", ConsoleColor.DarkGray);
                Console.Out.WriteColored((_showResultType ? "on" : "off"), _showResultType ? ConsoleColor.DarkGreen : ConsoleColor.DarkRed);
                Console.Out.WriteLineColored("]", ConsoleColor.DarkGray);
                return true;

            case "cls":
                Console.Clear();
                return true;

            case "reset":
                _previousCompilation = null;
                return true;

            case "exit":
                return false;

            default:
                Console.Out.WriteLineColored("[unknown command]", ConsoleColor.DarkRed);
                return true;

        }
    }

    protected override void Evaluate(string input)
    {
        var syntaxTree = SyntaxTree.Parse(input.AsMemory());

        var compilation = new Compilation(syntaxTree, _previousCompilation);

        if (_showTree)
            syntaxTree.WriteTo(Console.Out);

        if (_showProgram)
            compilation.WriteTo(Console.Out);

        var result = compilation.Evaluate(_symbols);

        var diagnostics = result.Diagnostics;

        Console.ResetColor();

        if (!diagnostics.Any())
        {
            if (result.Value is not null)
            {
                Console.Out.WriteColored(result.Value, ConsoleColor.White);
                if (_showResultType)
                    Console.Out.WriteColored($" ({TypeSymbol.TypeOf(result.Value)})", ConsoleColor.DarkGray);
                Console.WriteLine();
            }
            _previousCompilation = compilation;
        }
        else
        {
            foreach (var diagnostic in diagnostics)
            {
                var lineIndex = syntaxTree.Text.GetLineIndex(diagnostic.Span.Start);
                var lineNumber = lineIndex + 1;
                var line = syntaxTree.Text.Lines[lineIndex];
                var columnIndex = diagnostic.Span.Start - line.Start;
                var columnNumber = columnIndex + 1;
                var diagnosticColor = diagnostic.IsError ? ConsoleColor.DarkRed : ConsoleColor.DarkYellow;

                Console.WriteLine();

                Console.Out.WriteLineColored($"({lineNumber}, {columnNumber}): {diagnostic}", diagnosticColor);

                var prefixSpan = TextSpan.FromBounds(line.Start, diagnostic.Span.Start);
                var suffixSpan = TextSpan.FromBounds(diagnostic.Span.End, line.End);

                var prefix = syntaxTree.Text[prefixSpan];
                var error = syntaxTree.Text[diagnostic.Span];
                var suffix = syntaxTree.Text[suffixSpan];

                Console.Write("    ");
                Console.Write(prefix.ToString());
                Console.Out.WriteColored(error.ToString(), diagnosticColor);
                Console.Write(suffix.ToString());
                Console.WriteLine();

                Console.Write(new string(' ', 4 + columnIndex));
                Console.Write(new string('˄', error.Length));
                Console.WriteLine();
            }
            Console.WriteLine();
        }
    }
}