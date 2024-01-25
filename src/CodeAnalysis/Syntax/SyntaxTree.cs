using CodeAnalysis.Text;
using Spectre.Console;

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

    public static IReadOnlyList<Token> Scan(Source source, bool skipEof = true)
    {
        // A little hack, as we want to go through SyntaxTree to actually get the tokens..
        List<Token> tokens = [];

        CompilationUnit ParseTokens(SyntaxTree syntaxTree)
        {
            tokens = [.. Scanner.Scan(syntaxTree)];
            if (skipEof && tokens is [.., var last] && last.TokenKind is TokenKind.EOF)
            {
                tokens.Remove(last);
                return new CompilationUnit(syntaxTree, [], last);
            }

            return new CompilationUnit(syntaxTree, [], new Token(syntaxTree, TokenKind.EOF, new Range(source.Text.Length, source.Text.Length), TokenTrivia.Empty, "\0"));
        }

        // Evaluate the syntax tree.
        _ = new SyntaxTree(source, ParseTokens);
        return tokens;
    }
}