namespace CodeAnalysis.Syntax;

public sealed record class AssignmentExpression(Token Identifier, Token Equal, Expression Expression) : Expression(SyntaxNodeKind.AssignmentExpression)
{
    public override T Accept<T>(ISyntaxExpressionVisitor<T> visitor) => visitor.Visit(this);
    public override IEnumerable<SyntaxNode> GetChildren()
    {
        yield return Identifier;
        yield return Equal;
        yield return Expression;
    }
}
