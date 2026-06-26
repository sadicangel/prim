using System.Runtime.CompilerServices;
using CodeAnalysis.Diagnostics;
using CodeAnalysis.Text;

namespace CodeAnalysis.Syntax;

// internal readonly record struct ParseOptions(bool IsScript);

public sealed class SyntaxTree(SourceText sourceText)
{
    private readonly SourceText _sourceText = sourceText ?? throw new ArgumentNullException(nameof(sourceText));
    private readonly Parser _parser = new(sourceText);

    public CompilationUnitSyntax CompilationUnit => field ??= _parser.Parse();
    public IEnumerable<Diagnostic> Diagnostics => _parser.Diagnostics;

    internal Dictionary<SyntaxNode, SyntaxNode?> NodeParents { get => field ??= NodeParentsHelper.CreateNodeParents(CompilationUnit); }
    internal SyntaxNode? GetParent(SyntaxNode node) => NodeParents[node];
}

file static class NodeParentsHelper
{
    private static readonly EqualityComparer<SyntaxNode> s_comparer =
        EqualityComparer<SyntaxNode>.Create(ReferenceEquals, RuntimeHelpers.GetHashCode);

    public static Dictionary<SyntaxNode, SyntaxNode?> CreateNodeParents(CompilationUnitSyntax root)
    {
        var result = new Dictionary<SyntaxNode, SyntaxNode?>(s_comparer) { [root] = null };
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
