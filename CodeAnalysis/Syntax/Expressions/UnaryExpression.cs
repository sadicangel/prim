namespace CodeAnalysis.Syntax.Expressions;

public sealed record class UnaryExpression(SyntaxTree SyntaxTree, Token Operator, Expression Operand)
    : Expression(SyntaxNodeKind.UnaryExpression, SyntaxTree)
{
    public override T Accept<T>(ISyntaxExpressionVisitor<T> visitor) => visitor.Visit(this);
    public override IEnumerable<SyntaxNode> Children()
    {
        yield return Operator;
        yield return Operand;
    }
    public override string ToString() => base.ToString();
}
