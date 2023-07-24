using CodeAnalysis;
using CodeAnalysis.Symbols;
using CodeAnalysis.Syntax;
using CodeAnalysis.Text;

namespace Repl;
internal sealed class PrimRepl : ReplBase
{
    private readonly Dictionary<Symbol, object?> _globals;
    private bool _showTree;
    private bool _showProgram;
    private bool _showResultType = true;
    private Compilation? _previousCompilation;

    private PrimRepl() : base(commandPrefix: "\\")
    {
        _globals = new Dictionary<Symbol, object?>();
        _showTree = false;
        _showProgram = false;
        Console.ForegroundColor = ConsoleColor.White;
    }

    public static void Start() => new PrimRepl().Run();

    protected override object? RenderLine(IReadOnlyList<string> lines, int lineIndex, object? state)
    {
        if (state is not SyntaxTree syntaxTree)
            syntaxTree = SyntaxTree.Parse(String.Join(Environment.NewLine, lines));

        var lineSpan = syntaxTree.Text.Lines[lineIndex].Span;
        var classifiedSpans = Classifier.Classify(syntaxTree, lineSpan);

        foreach (var classifiedSpan in classifiedSpans)
        {
            var tokenText = syntaxTree.Text[classifiedSpan.Span];

            var color = classifiedSpan.Classification switch
            {
                Classification.Number => ConsoleColor.Cyan,
                Classification.Keyword => ConsoleColor.DarkBlue,
                Classification.Comment => ConsoleColor.DarkGreen,
                Classification.String => ConsoleColor.Magenta,
                Classification.Identifier => ConsoleColor.Blue,
                _ => ConsoleColor.DarkGray,
            };

            Console.Out.WriteColored(tokenText, color);
        }
        return state;
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

        if (!diagnostics.HasErrors)
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

        diagnostics.WriteTo(Console.Out);
    }
}

internal sealed class Classifier
{
    private Classifier() { }

    public static IReadOnlyList<ClassifiedSpan> Classify(SyntaxTree syntaxTree, TextSpan span)
    {
        var list = new List<ClassifiedSpan>();
        ClassifyNode(syntaxTree.Root, span, list);
        return list;

        static void ClassifyNode(SyntaxNode node, TextSpan span, List<ClassifiedSpan> classifiedSpans)
        {
            if (!node.FullSpan.OverlapsWith(span))
                return;

            if (node is Token token)
                ClassifyToken(token, span, classifiedSpans);

            foreach (var child in node.Children())
                ClassifyNode(child, span, classifiedSpans);

            static void ClassifyToken(Token token, TextSpan span, List<ClassifiedSpan> classifiedSpans)
            {
                foreach (var trivia in token.LeadingTrivia)
                    AddClassification(trivia.TokenKind, trivia.Span, span, classifiedSpans);

                AddClassification(token.TokenKind, token.Span, span, classifiedSpans);

                foreach (var trivia in token.TrailingTrivia)
                    AddClassification(trivia.TokenKind, trivia.Span, span, classifiedSpans);
            }

            static void AddClassification(TokenKind elementKind, TextSpan elementSpan, TextSpan span, List<ClassifiedSpan> classifiedSpans)
            {
                if (!elementSpan.OverlapsWith(span))
                    return;

                var adjustedStart = Math.Max(elementSpan.Start, span.Start);
                var adjustedEnd = Math.Min(elementSpan.End, span.End);
                var adjustedSpan = TextSpan.FromBounds(adjustedStart, adjustedEnd);
                var classification = GetClassification(elementKind);
                var classifiedSpan = new ClassifiedSpan(adjustedSpan, classification);
                classifiedSpans.Add(classifiedSpan);

                static Classification GetClassification(TokenKind kind) => kind switch
                {
                    TokenKind when kind.IsNumber() => Classification.Number,
                    TokenKind when kind.IsKeyword() => Classification.Keyword,
                    TokenKind when kind.IsComment() => Classification.Comment,
                    TokenKind.String => Classification.String,
                    TokenKind.Identifier => Classification.Identifier,
                    _ => Classification.Text,
                };
            }
        }
    }
}

internal enum Classification
{
    Number,
    Keyword,
    Comment,
    String,
    Identifier,
    Text
}

internal sealed record class ClassifiedSpan(TextSpan Span, Classification Classification);