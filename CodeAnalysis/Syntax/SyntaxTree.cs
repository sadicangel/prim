using CodeAnalysis.Text;

namespace CodeAnalysis.Syntax;

public sealed record class SyntaxTree(SourceText Text, IEnumerable<Diagnostic> Diagnostics, CompilationUnit Root) : INode
{
    TextSpan INode.Span { get => ((INode)Root).Span; }

    public void WriteTo(TextWriter writer, string indent = "", bool isLast = true) => Root.WriteTo(writer, indent, isLast);
    IEnumerable<INode> INode.GetChildren() => ((INode)Root).GetChildren();

    public static SyntaxTree Parse(string text) => Parse(new SourceText(text));
    public static SyntaxTree Parse(SourceText text)
    {
        var parser = new Parser(text);
        var compilationUnit = parser.ParseCompilationUnit();
        return new SyntaxTree(text, parser.Diagnostics, compilationUnit);
    }

    public static IEnumerable<Token> ParseTokens(string text) => ParseTokens(new SourceText(text));
    public static IEnumerable<Token> ParseTokens(SourceText text)
    {
        var lexer = new Lexer(text);
        while (true)
        {
            var token = lexer.NextToken();
            if (token.Kind == TokenKind.EOF)
                break;
            yield return token;
        }
    }

    public override string ToString()
    {
        using var writer = new StringWriter();
        WriteTo(writer);
        return writer.ToString();
    }
}