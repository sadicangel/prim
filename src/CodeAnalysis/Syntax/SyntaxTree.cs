using System.Runtime.CompilerServices;
using CodeAnalysis.Diagnostics;
using CodeAnalysis.Parsing;
using CodeAnalysis.Scanning;
using CodeAnalysis.Text;

namespace CodeAnalysis.Syntax;

public sealed record class SyntaxTree(
    SourceText SourceText,
    bool IsScript,
    CompilationUnitSyntax CompilationUnit,
    DiagnosticBag Diagnostics)
    : IEquatable<SyntaxTree>
{
    private Dictionary<SyntaxNode, SyntaxNode?>? _nodeParents;

    private SyntaxTree()
        : this(new SourceText(""), IsScript: false, CompilationUnit: null!, Diagnostics: [])
    {
        CompilationUnit = new CompilationUnitSyntax(this, [], SyntaxFactory.SyntheticToken(SyntaxKind.EofToken, this));
    }

    private SyntaxTree(SourceText sourceText, bool isScript)
        : this(sourceText, isScript, CompilationUnit: null!, Diagnostics: [])
    {
        // TODO: Make this lazy evaluated?
        CompilationUnit = Parser.Parse(this);
    }

    internal static SyntaxTree Empty { get; } = new();

    public bool Equals(SyntaxTree? other) => ReferenceEquals(this, other);
    public override int GetHashCode() => RuntimeHelpers.GetHashCode(this);

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
        var syntaxTokens = Lexer.Scan(syntaxTree);
        return [.. syntaxTokens];
    }

    public static SyntaxTree Parse(SourceText sourceText) => new(sourceText, isScript: false);

    public static SyntaxTree ParseScript(SourceText sourceText) => new(sourceText, isScript: true);

    private readonly record struct ParseOptions(bool IsScript);
}
