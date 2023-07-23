namespace CodeAnalysis.Syntax.Statements;

public sealed record class ContinueStatement(SyntaxTree SyntaxTree, Token Continue, Token Semicolon)
    : Statement(SyntaxNodeKind.ContinueStatement, SyntaxTree)
{
    public override T Accept<T>(ISyntaxStatementVisitor<T> visitor) => visitor.Visit(this);
    public override IEnumerable<SyntaxNode> GetChildren()
    {
        yield return Continue;
        yield return Semicolon;
    }
    public override string ToString() => base.ToString();
}