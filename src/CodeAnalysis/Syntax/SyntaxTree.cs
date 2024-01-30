using CodeAnalysis.Text;
using Spectre.Console;
using System.Diagnostics;

namespace CodeAnalysis.Syntax;

public sealed record class SyntaxTree(Source Source, CompilationUnit Root)
    : INode
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
    IEnumerable<INode> INode.Children() => ((INode)Root).Children();

    public void WriteTo(TreeNode root) => Root.WriteTo(root);

    public static IReadOnlyList<Token> Scan(Source source)
    {
        static CompilationUnit Parse(SyntaxTree syntaxTree)
        {
            var tokens = new List<Token>(Scanner.Scan(syntaxTree));
            var eof = tokens is [.., var last] && last.TokenKind is TokenKind.EOF
                ? last
                : new Token(syntaxTree, TokenKind.EOF, new Range(syntaxTree.Source.Text.Length, syntaxTree.Source.Text.Length), TokenTrivia.Empty, "\0");

            return new CompilationUnit(syntaxTree, tokens, eof);
        }

        return [.. new SyntaxTree(source, Parse).Root.Nodes.Cast<Token>()];
    }

    public static SyntaxTree Parse(Source source) => new(source, Parser.ParseCompilationUnit);
    public static SyntaxTree ParseScript(Source source) => new(source, Parser.ParseScript);
}