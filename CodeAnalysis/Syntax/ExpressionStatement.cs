namespace CodeAnalysis.Syntax;

public sealed record class ExpressionStatement(Expression Expression, Token? Semicolon = null) : Statement(SyntaxNodeKind.ExpressionStatement)
{
    public override T Accept<T>(ISyntaxStatementVisitor<T> visitor) => visitor.Visit(this);
    public override IEnumerable<SyntaxNode> GetChildren()
    {
        yield return Expression;
        if (Semicolon is not null)
            yield return Semicolon;
    }
}
