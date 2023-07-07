namespace CodeAnalysis.Syntax;

public sealed record class UnaryExpression(SyntaxTree SyntaxTree, Token Operator, Expression Operand)
    : Expression(SyntaxNodeKind.UnaryExpression, SyntaxTree)
{
    public override T Accept<T>(ISyntaxExpressionVisitor<T> visitor) => visitor.Visit(this);
    public override IEnumerable<SyntaxNode> GetChildren()
    {
        yield return Operator;
        yield return Operand;
    }
}
