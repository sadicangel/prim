using CodeAnalysis.Text;
using System.Runtime.CompilerServices;

namespace CodeAnalysis.Syntax;

public sealed record class SyntaxTree(SourceText Text, CompilationUnit Root, IEnumerable<Diagnostic> Diagnostics) : INode
{
    public void WriteTo(TextWriter writer, string indent = "", bool isLast = true) => Root.WriteTo(writer, indent, isLast);
    IEnumerable<INode> INode.GetChildren() => ((INode)Root).GetChildren();

    private SyntaxTree(SourceText text, Func<SyntaxTree, ParseResult> parse) : this(text, null!, null!)
    {
        (Root, Diagnostics) = parse.Invoke(this);
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
        ParseResult ParseTokens(SyntaxTree syntaxTree)
        {
            (tokens, var diagnostics) = Lexer.Lex(syntaxTree, includeEof ? static (ref Token t) => true : static (ref Token t) => t.TokenKind is not TokenKind.EOF);
            var eof = tokens.Count > 0 ? tokens[^1] : new Token(syntaxTree, TokenKind.EOF, Position: 0, Text: "", Array.Empty<Trivia>(), Array.Empty<Trivia>());
            return new ParseResult(new CompilationUnit(syntaxTree, Array.Empty<GlobalSyntaxNode>(), eof), diagnostics);
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