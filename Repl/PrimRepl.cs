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

    public PrimRepl() : base(commandPrefix: "\\")
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

    [Command("tree", Description = "Show/hide parsed tree")]
    private void TreeCommand()
    {
        _showTree = !_showTree;
        Console.Out.WriteColored("[show parse tree: ", ConsoleColor.DarkGray);
        Console.Out.WriteColored((_showTree ? "on" : "off"), _showTree ? ConsoleColor.DarkGreen : ConsoleColor.DarkRed);
        Console.Out.WriteLineColored("]", ConsoleColor.DarkGray);
    }

    [Command("program", Description = "Show/hide parsed program")]
    private void ProgramCommand()
    {
        _showProgram = !_showProgram;
        Console.Out.WriteColored("[show bound tree: ", ConsoleColor.DarkGray);
        Console.Out.WriteColored((_showProgram ? "on" : "off"), _showProgram ? ConsoleColor.DarkGreen : ConsoleColor.DarkRed);
        Console.Out.WriteLineColored("]", ConsoleColor.DarkGray);
    }

    [Command("type", Description = "Show/hide result type")]
    private void TypeCommand()
    {
        _showResultType = !_showResultType;
        Console.Out.WriteColored("[show result type: ", ConsoleColor.DarkGray);
        Console.Out.WriteColored((_showResultType ? "on" : "off"), _showResultType ? ConsoleColor.DarkGreen : ConsoleColor.DarkRed);
        Console.Out.WriteLineColored("]", ConsoleColor.DarkGray);
    }

    [Command("reset", Description = "Reset REPL")]
    private void ResetCommand()
    {
        _previousCompilation = null;
    }

    [Command("load", Description = "Load script from file")]
    private void LoadCommand(string filename)
    {
        if (String.IsNullOrWhiteSpace(filename))
        {
            Console.Out.WriteColored($"'{filename}' is not a valid file name", ConsoleColor.DarkRed);
            return;
        }

        var path = Path.GetFullPath(filename);
        if (!File.Exists(path))
        {
            Console.Out.WriteColored($"Could not find file '{path}'", ConsoleColor.DarkRed);
            return;
        }

        Evaluate(SyntaxTree.Load(filename));
    }

    protected override void Evaluate(string input) => Evaluate(SyntaxTree.Parse(input));

    private void Evaluate(SyntaxTree syntaxTree)
    {
        var compilation = new Compilation(_previousCompilation, syntaxTree);

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
            diagnostics.WriteTo(Console.Out);
        }
    }
}