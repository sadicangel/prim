namespace CodeAnalysis.Syntax;

public sealed record class ContinueStatement(Token Continue, Token Semicolon) : Statement(SyntaxNodeKind.ContinueStatement)
{
    public override T Accept<T>(ISyntaxStatementVisitor<T> visitor) => visitor.Visit(this);
    public override IEnumerable<SyntaxNode> GetChildren()
    {
        yield return Continue;
        yield return Semicolon;
    }
}