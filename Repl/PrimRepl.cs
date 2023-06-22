using CodeAnalysis;
using CodeAnalysis.Symbols;
using CodeAnalysis.Syntax;
using CodeAnalysis.Text;

namespace Repl;
internal sealed class PrimRepl : ReplBase
{
    private readonly Dictionary<VariableSymbol, object> _variables;
    private bool _showTree;
    private bool _showProgram;
    private Compilation? _previousCompilation;

    public PrimRepl()
    {
        _variables = new Dictionary<VariableSymbol, object>();
        _showTree = false;
        _showProgram = false;
    }

    protected override void RenderLine(string line)
    {
        var tokens = SyntaxTree.ParseTokens(line.AsMemory());
        foreach (var token in tokens)
        {
            var color = ConsoleColor.DarkGray;
            if (token.Kind.IsLiteral())
                color = ConsoleColor.Cyan;
            else if (token.Kind == TokenKind.Identifier)
                color = ConsoleColor.DarkYellow;
            else if (token.Kind.IsKeyword())
                color = ConsoleColor.Blue;

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

        var result = compilation.Evaluate(_variables);

        var diagnostics = result.Diagnostics;

        Console.ResetColor();

        if (!diagnostics.Any())
        {
            Console.Out.WriteLineColored(result.Value, ConsoleColor.Magenta);
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