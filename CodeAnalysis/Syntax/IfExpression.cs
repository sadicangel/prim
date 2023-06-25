namespace CodeAnalysis.Syntax;

public sealed record class IfExpression(Token IfToken, Expression Condition, Expression Then, Token ElseToken, Expression Else) : Expression(SyntaxNodeKind.IfExpression)
{
    public override T Accept<T>(ISyntaxExpressionVisitor<T> visitor) => visitor.Visit(this);
    public override IEnumerable<SyntaxNode> GetChildren()
    {
        yield return IfToken;
        yield return Condition;
        yield return Then;
        yield return ElseToken;
        yield return Else;
    }
}