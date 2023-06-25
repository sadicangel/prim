namespace CodeAnalysis.Syntax;

public sealed record class AssignmentExpression(Token IdentifierToken, Token EqualsToken, Expression Expression) : Expression(SyntaxNodeKind.AssignmentExpression)
{
    public override T Accept<T>(ISyntaxExpressionVisitor<T> visitor) => visitor.Visit(this);
    public override IEnumerable<SyntaxNode> GetChildren()
    {
        yield return IdentifierToken;
        yield return EqualsToken;
        yield return Expression;
    }
}
