using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using CodeAnalysis.Diagnostics;
using CodeAnalysis.Parsing;
using CodeAnalysis.Text;

namespace CodeAnalysis.Syntax;

public readonly record struct ParseOptions(bool IsScript);

public sealed class SyntaxTree(SourceText sourceText, ParseOptions parseOptions = default)
{
    private readonly Lazy<Result<CompilationUnitSyntax>> _parseResult = new(() => Parser.Parse(sourceText));

    public SourceText SourceText { get; } = sourceText ?? throw new ArgumentNullException(nameof(sourceText));
    public ParseOptions ParseOptions { get; } = parseOptions;
    public CompilationUnitSyntax CompilationUnit => _parseResult.Value.Value;
    public ImmutableArray<Diagnostic> Diagnostics => _parseResult.Value.Diagnostics;

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
