using CodeAnalysis.Text;
using Spectre.Console;

namespace CodeAnalysis.Syntax;

public sealed record class SyntaxTree(Source Source, CompilationUnit Root)
    : INode
{
    private Dictionary<SyntaxNode, SyntaxNode?>? _nodeParents;

    public DiagnosticBag Diagnostics { get; } = new DiagnosticBag();

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
}