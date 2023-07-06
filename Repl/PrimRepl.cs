using CodeAnalysis;
using CodeAnalysis.Symbols;
using CodeAnalysis.Syntax;

namespace Repl;
internal sealed class PrimRepl : ReplBase
{
    private readonly Dictionary<Symbol, object?> _globals;
    private bool _showTree;
    private bool _showProgram;
    private bool _showResultType = true;
    private Compilation? _previousCompilation;

    public PrimRepl()
    {
        _globals = new Dictionary<Symbol, object?>();
        _showTree = false;
        _showProgram = false;
        Console.ForegroundColor = ConsoleColor.White;
    }

    protected override void RenderLine(string line)
    {
        var tokens = SyntaxTree.ParseTokens(line);
        foreach (var token in tokens)
        {
            var color = token.TokenKind switch
            {
                TokenKind k when k.IsNumber() => ConsoleColor.Cyan,
                TokenKind.String => ConsoleColor.Magenta,
                TokenKind.Identifier => ConsoleColor.Blue,
                TokenKind k when k.IsKeyword() => ConsoleColor.DarkBlue,
                _ => ConsoleColor.DarkGray,
            };

            Console.Out.WriteColored(token.Text, color);
        }
    }

    protected override bool IsCompleteInput(string text)
    {
        if (text.Length == 0 || LastTwoLinesAreBlank(text))
            return true;

        var syntaxTree = SyntaxTree.Parse(text);

        // We need to skip EOF.
        if (syntaxTree.Root.Nodes.Count == 0 || syntaxTree.Root.Nodes[^1].GetLastToken().IsMissing)
            return false;

        return true;

        static bool LastTwoLinesAreBlank(ReadOnlySpan<char> text)
        {
            var index = text.LastIndexOf(Environment.NewLine);
            if (index >= 0)
            {
                text = text[..index];
                if (text.IsWhiteSpace())
                {
                    index = text.LastIndexOf(Environment.NewLine);
                    if (index >= 0)
                    {
                        text = text[..index];
                        if (text.IsWhiteSpace())
                            return true;
                    }
                }
            }
            return false;
        }
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
        var syntaxTree = SyntaxTree.Parse(input);

        var compilation = new Compilation(syntaxTree, _previousCompilation);

        if (_showTree)
            syntaxTree.WriteTo(Console.Out);

        if (_showProgram)
            compilation.WriteTo(Console.Out);

        var result = compilation.Evaluate(_globals);

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
            diagnostics.WriteTo(Console.Out, syntaxTree);
        }
    }
}