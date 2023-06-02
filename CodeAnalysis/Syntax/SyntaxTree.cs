namespace CodeAnalysis.Syntax;

public sealed record class SyntaxTree(IEnumerable<Diagnostic> Diagnostics, Expression Root, Token Eof) : IPrintableNode
{
    public void PrettyPrint(TextWriter writer, string indent = "", bool isLast = true) => Root.PrettyPrint(writer, indent, isLast);
    IEnumerable<IPrintableNode> IPrintableNode.GetChildren() => Root.GetChildren();

    public static SyntaxTree Parse(string text) => new Parser(text).Parse();

    public static IEnumerable<Token> ParseTokens(string text)
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
}