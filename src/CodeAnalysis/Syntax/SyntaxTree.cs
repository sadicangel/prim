using CodeAnalysis.Diagnostics;
using CodeAnalysis.Parsing;
using CodeAnalysis.Syntax.Parsing;
using CodeAnalysis.Text;

namespace CodeAnalysis.Syntax;

public sealed class SyntaxTree
{
    private Dictionary<SyntaxNode, SyntaxNode?>? _nodeParents;

    internal SyntaxTree(SourceText sourceText, Func<SyntaxTree, CompilationUnitSyntax> getRoot)
    {
        SourceText = sourceText;
        Root = getRoot.Invoke(this);
    }

    public SourceText SourceText { get; }
    public CompilationUnitSyntax Root { get; }
    public DiagnosticBag Diagnostics { get; init; } = [];

    public override string ToString() => $"SyntaxTree {{ Root = {Root} }}";

    internal SyntaxNode? GetParent(SyntaxNode node)
    {
        if (_nodeParents is null)
            Interlocked.CompareExchange(ref _nodeParents, CreateNodeParents(Root), null);

        return _nodeParents[node];

        static Dictionary<SyntaxNode, SyntaxNode?> CreateNodeParents(CompilationUnitSyntax root)
        {
            var result = new Dictionary<SyntaxNode, SyntaxNode?> { [root] = null };
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

    public static SyntaxList<SyntaxToken> Scan(SourceText sourceText)
    {
        static CompilationUnitSyntax Parse(SyntaxTree syntaxTree)
        {
            var tokens = new SyntaxList<SyntaxNode>.Builder();
            tokens.AddRange(Scanner.Scan(syntaxTree));
            var eof = tokens is [.., SyntaxToken last and { SyntaxKind: SyntaxKind.EofToken }]
                ? last
                : SyntaxFactory.EofToken(syntaxTree);

            return new CompilationUnitSyntax(syntaxTree, tokens.ToSyntaxList(), eof);
        }

        return new SyntaxList<SyntaxToken>([.. new SyntaxTree(sourceText, Parse).Root.SyntaxNodes.Cast<SyntaxToken>()]);
    }

    public static SyntaxTree ParseScript(SourceText sourceText) =>
        new(sourceText, syntaxTree => Parser.Parse(syntaxTree, isScript: true));
}
