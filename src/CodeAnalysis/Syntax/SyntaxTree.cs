using CodeAnalysis.Diagnostics;
using CodeAnalysis.Parsing;
using CodeAnalysis.Text;

namespace CodeAnalysis.Syntax;

public sealed class SyntaxTree
{
    private Dictionary<SyntaxNode, SyntaxNode?>? _nodeParents;

    private SyntaxTree(SourceText sourceText, bool isScript)
    {
        SourceText = sourceText;
        IsScript = isScript;
        // TODO: Make this lazy evaluated?
        CompilationUnit = Parser.Parse(this);
    }

    public SourceText SourceText { get; }
    public bool IsScript { get; }
    public CompilationUnitSyntax CompilationUnit { get; }
    public DiagnosticBag Diagnostics { get; init; } = [];

    public override string ToString() => $"SyntaxTree {{ CompilationUnit = {CompilationUnit} }}";

    internal SyntaxNode? GetParent(SyntaxNode node)
    {
        if (_nodeParents is null)
            Interlocked.CompareExchange(ref _nodeParents, CreateNodeParents(CompilationUnit), null);

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
        var syntaxTree = new SyntaxTree(sourceText, isScript: false);
        var syntaxTokens = Scanner.Scan(syntaxTree);
        return [.. syntaxTokens];
    }

    public static SyntaxTree Parse(SourceText sourceText) => new(sourceText, isScript: false);

    public static SyntaxTree ParseScript(SourceText sourceText) => new(sourceText, isScript: true);

    private readonly record struct ParseOptions(bool IsScript);
}
