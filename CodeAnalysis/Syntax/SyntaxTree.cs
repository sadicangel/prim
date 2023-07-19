using CodeAnalysis.Text;

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

    public static SyntaxTree Parse(string text) => new(new SourceText(text), Parser.Parse);

    public static IReadOnlyList<Token> ParseTokens(string text) => ParseTokens(new SourceText(text));
    public static IReadOnlyList<Token> ParseTokens(SourceText text)
    {
        // A little hack, as we want to go through SyntaxTree to actually get the tokens..
        IReadOnlyList<Token> tokens = Array.Empty<Token>();
        ParseResult ParseTokens(SyntaxTree syntaxTree)
        {
            (tokens, var diagnostics) = Lexer.Lex(syntaxTree, static t => t.TokenKind is not TokenKind.EOF);
            var eof = tokens.Count > 0 ? tokens[^1] : new Token(syntaxTree, TokenKind.EOF, Position: 0, Text: "");
            return new ParseResult(new CompilationUnit(syntaxTree, Array.Empty<GlobalSyntaxNode>(), eof), diagnostics);
        }
        _ = new SyntaxTree(text, ParseTokens);
        return tokens;
    }

    public override string ToString()
    {
        using var writer = new StringWriter();
        WriteTo(writer);
        return writer.ToString();
    }
}