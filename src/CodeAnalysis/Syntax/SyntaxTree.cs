using CodeAnalysis.Diagnostics;
using CodeAnalysis.Parsing;
using CodeAnalysis.Scanning;
using CodeAnalysis.Text;

namespace CodeAnalysis.Syntax;

public readonly record struct ParseOptions(bool IsScript);

public sealed class SyntaxTree(SourceText sourceText, ParseOptions parseOptions = default)
{
    public SourceText SourceText { get; } = sourceText ?? throw new ArgumentNullException(nameof(sourceText));
    public ParseOptions ParseOptions { get; } = parseOptions;
    public DiagnosticBag Diagnostics { get; } = [];
    public CompilationUnitSyntax CompilationUnit { get => field ??= Parser.Parse(this); }

    internal Dictionary<SyntaxNode, SyntaxNode?> NodeParents { get => field ??= NodeParentsHelper.CreateNodeParents(CompilationUnit); }

    internal SyntaxNode? GetParent(SyntaxNode node) => NodeParents[node];

    public static SyntaxList<SyntaxToken> Scan(SourceText sourceText)
    {
        var syntaxTree = new SyntaxTree(sourceText);
        var syntaxTokens = Scanner.Scan(syntaxTree);
        return [.. syntaxTokens];
    }
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
