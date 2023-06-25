namespace CodeAnalysis.Syntax;

public sealed record class ExpressionStatement(Expression Expression, Token? SemicolonToken = null) : Statement(SyntaxNodeKind.ExpressionStatement)
{
    public override T Accept<T>(ISyntaxStatementVisitor<T> visitor) => visitor.Visit(this);
    public override IEnumerable<SyntaxNode> GetChildren()
    {
        yield return Expression;
        if (SemicolonToken is not null)
            yield return SemicolonToken;
    }
}
