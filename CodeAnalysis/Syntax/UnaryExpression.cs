namespace CodeAnalysis.Syntax;

public sealed record class UnaryExpression(Token Operator, Expression Operand) : Expression(SyntaxNodeKind.UnaryExpression)
{
    public override T Accept<T>(ISyntaxExpressionVisitor<T> visitor) => visitor.Visit(this);
    public override IEnumerable<SyntaxNode> GetChildren()
    {
        yield return Operator;
        yield return Operand;
    }
}
