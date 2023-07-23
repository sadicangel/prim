using CodeAnalysis.Text;
using System.Runtime.CompilerServices;

namespace CodeAnalysis.Syntax;

public sealed record class SyntaxTree(SourceText Text, CompilationUnit Root, IEnumerable<Diagnostic> Diagnostics) : INode
{
    private Dictionary<SyntaxNode, SyntaxNode?>? _nodeParents;

    public void WriteTo(TextWriter writer, string indent = "", bool isLast = true) => Root.WriteTo(writer, indent, isLast);
    IEnumerable<INode> INode.Children() => ((INode)Root).Children();

    private SyntaxTree(SourceText text, Func<SyntaxTree, AnalysisResult<CompilationUnit>> parse) : this(text, null!, null!)
    {
        (Root, Diagnostics) = parse.Invoke(this);
    }

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

    public static SyntaxTree Load(string fileName)
    {
        var text = File.ReadAllText(fileName);
        var sourceText = new SourceText(text, fileName);
        return new(sourceText, Parser.Parse);
    }

    public static SyntaxTree Parse(string text) => Parse(new SourceText(text));

    public static SyntaxTree Parse(SourceText text) => new(text, Parser.Parse);

    public static IReadOnlyList<Token> ParseTokens(string text, bool includeEof = false)
    {
        // A little hack, as we want to go through SyntaxTree to actually get the tokens..
        IReadOnlyList<Token> tokens = Array.Empty<Token>();
        AnalysisResult<CompilationUnit> ParseTokens(SyntaxTree syntaxTree)
        {
            (tokens, var diagnostics) = Lexer.Lex(syntaxTree, includeEof ? static (ref Token t) => true : static (ref Token t) => t.TokenKind is not TokenKind.EOF);
            var eof = tokens.Count > 0 ? tokens[^1] : new Token(syntaxTree, TokenKind.EOF, Position: 0, Text: "", Array.Empty<Trivia>(), Array.Empty<Trivia>());
            return new AnalysisResult<CompilationUnit>(new CompilationUnit(syntaxTree, Array.Empty<GlobalSyntaxNode>(), eof), diagnostics);
        }
        _ = new SyntaxTree(new SourceText(text), ParseTokens);
        return tokens;
    }

    public override string ToString()
    {
        using var writer = new StringWriter();
        WriteTo(writer);
        return writer.ToString();
    }

    public bool Equals(SyntaxTree? other) => ReferenceEquals(this, other);

    public override int GetHashCode() => RuntimeHelpers.GetHashCode(this);
}