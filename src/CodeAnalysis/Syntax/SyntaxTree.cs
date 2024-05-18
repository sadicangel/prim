using CodeAnalysis.Diagnostics;
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
}