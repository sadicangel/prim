namespace CodeAnalysis.Syntax;

public sealed record class ExpressionStatement(Expression Expression, Token? SemicolonToken = null) : Statement(NodeKind.ExpressionStatement)
{
    public override T Accept<T>(IStatementVisitor<T> visitor) => visitor.Accept(this);
    public override IEnumerable<Node> GetChildren()
    {
        yield return Expression;
        if (SemicolonToken is not null)
            yield return SemicolonToken;
    }
}
