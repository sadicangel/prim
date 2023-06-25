namespace CodeAnalysis.Syntax;

public sealed record class UnaryExpression(Token OperatorToken, Expression Operand) : Expression(SyntaxNodeKind.UnaryExpression)
{
    public override T Accept<T>(ISyntaxExpressionVisitor<T> visitor) => visitor.Visit(this);
    public override IEnumerable<SyntaxNode> GetChildren()
    {
        yield return OperatorToken;
        yield return Operand;
    }
}
