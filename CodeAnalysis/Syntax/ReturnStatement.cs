namespace CodeAnalysis.Syntax;

public sealed record class ReturnStatement(Token Return, Expression? Expression, Token Semicolon) : Statement(SyntaxNodeKind.ReturnStatement)
{
    public ReturnStatement(Token @return, Token semicolon) : this(@return, Expression: null, semicolon) { }

    public override T Accept<T>(ISyntaxStatementVisitor<T> visitor) => visitor.Visit(this);
    public override IEnumerable<SyntaxNode> GetChildren()
    {
        yield return Return;
        if (Expression is not null)
            yield return Expression;
        yield return Semicolon;
    }
}