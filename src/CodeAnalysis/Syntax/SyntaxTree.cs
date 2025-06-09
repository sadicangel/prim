using System.Collections.Immutable;
using CodeAnalysis.Diagnostics;
using CodeAnalysis.Parsing;
using CodeAnalysis.Text;

namespace CodeAnalysis.Syntax;

public readonly record struct ParseOptions(bool IsScript);

public sealed class SyntaxTree(SourceText sourceText, ParseOptions parseOptions = default)
{
    internal static SyntaxTree Empty { get; } = new SyntaxTree(SourceText.Empty);

    public SourceText SourceText { get; } = sourceText ?? throw new ArgumentNullException(nameof(sourceText));
    public ParseOptions ParseOptions { get; } = parseOptions;

    internal ParseResult ParseResult => field ??= Parser.Parse(this);
    public CompilationUnitSyntax CompilationUnit => ParseResult.CompilationUnit;
    public ImmutableArray<Diagnostic> Diagnostics => ParseResult.Diagnostics;

    internal Dictionary<SyntaxNode, SyntaxNode?> NodeParents { get => field ??= NodeParentsHelper.CreateNodeParents(CompilationUnit); }

    internal SyntaxNode? GetParent(SyntaxNode node) => NodeParents[node];
}

file static class NodeParentsHelper
{
    public static Dictionary<SyntaxNode, SyntaxNode?> CreateNodeParents(CompilationUnitSyntax root)
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
