using CodeAnalysis.Text;
using Spectre.Console;
using Spectre.Console.Rendering;
using System.Diagnostics;

namespace CodeAnalysis.Syntax;

public sealed record class SyntaxTree(Source Source, CompilationUnit Root)
{
    private Dictionary<SyntaxNode, SyntaxNode?>? _nodeParents;

    private SyntaxTree(Source source, Func<SyntaxTree, CompilationUnit> parse) : this(source, default(CompilationUnit)!)
    {
        Root = parse.Invoke(this);
    }

    public DiagnosticBag Diagnostics { get; init; } = [];

    internal SyntaxNode? GetParent(SyntaxNode node)
    {
        Debug.WriteLine("enter");
        if (_nodeParents is null)
            Interlocked.CompareExchange(ref _nodeParents, CreateNodeParents(Root), null);

        return _nodeParents[node];

        static Dictionary<SyntaxNode, SyntaxNode?> CreateNodeParents(CompilationUnit root)
        {
            var result = new Dictionary<SyntaxNode, SyntaxNode?>
            {
                [root] = null
            };
            CreateParentsDictionary(result, root);
            return result;

            static void CreateParentsDictionary(Dictionary<SyntaxNode, SyntaxNode?> result, SyntaxNode node)
            {
                foreach (var child in node.Children())
                {
                    result.Add(child, node);
                    CreateParentsDictionary(result, child);
                }
            }
        }
    }

    public IRenderable ToRenderable()
    {
        var tree = new Tree($"[aqua]{Root.NodeKind}[/]")
            .Style("dim white");

        WriteTo(Root, tree);

        return tree;

        static void WriteTo(SyntaxNode syntaxNode, IHasTreeNodes treeNode)
        {
            foreach (var child in syntaxNode.Children())
            {
                if (child is Token token)
                {
                    foreach (var trivia in token.Trivia.Leading)
                        treeNode.AddNode($"[grey66]L: {trivia.TokenKind}[/]");
                    if (token.Value is not null)
                        treeNode.AddNode($"[green3]{token.TokenKind}Token[/] {FormatLiteral(token)}");
                    else
                        treeNode.AddNode($"[green3]{token.TokenKind}Token[/]");
                    foreach (var trivia in token.Trivia.Trailing)
                        treeNode.AddNode($"[grey66]T: {trivia.TokenKind}[/]");
                }
                else
                {
                    WriteTo(child, treeNode.AddNode($"[aqua]{child.NodeKind}[/]"));
                }
            }
        }

        static string FormatLiteral(Token token)
        {
            return token.TokenKind switch
            {
                TokenKind.I32 => $"[gold3]{token.Value}[/]",
                TokenKind.F32 => $"[gold3]{token.Value}[/]",
                TokenKind.Str => $"[darkorange3]\"{token.Value}\"[/]",
                TokenKind.True => $"[blue3_1]{token.Value}[/]",
                TokenKind.False => $"[blue3_1]{token.Value}[/]",
                TokenKind.Null => $"[blue3_1]{token.Value}[/]",
                _ => throw new UnreachableException($"Unexpected {nameof(TokenKind)} '{token.TokenKind}'"),
            };
        }
    }

    public static IReadOnlyList<Token> Scan(Source source)
    {
        static CompilationUnit Parse(SyntaxTree syntaxTree)
        {
            var tokens = new List<Token>(Scanner.Scan(syntaxTree));
            var eof = tokens is [.., var last] && last.TokenKind is TokenKind.Eof
                ? last
                : new Token(syntaxTree, TokenKind.Eof, new Range(syntaxTree.Source.Text.Length, syntaxTree.Source.Text.Length), TokenTrivia.Empty, "\0");

            return new CompilationUnit(syntaxTree, tokens, eof);
        }

        return [.. new SyntaxTree(source, Parse).Root.Nodes.Cast<Token>()];
    }

    public static SyntaxTree Parse(Source source) => new(source, Parser.ParseCompilationUnit);
    public static SyntaxTree ParseScript(Source source) => new(source, Parser.ParseScript);
}