namespace CodeAnalysis.Syntax;

public sealed record class IfExpression(Token If, Token OpenParenthesis, Expression Condition, Token CloseParenthesis, Expression Then, Token ElseToken, Expression Else) : Expression(SyntaxNodeKind.IfExpression)
{
    public override T Accept<T>(ISyntaxExpressionVisitor<T> visitor) => visitor.Visit(this);
    public override IEnumerable<SyntaxNode> GetChildren()
    {
        yield return If;
        yield return OpenParenthesis;
        yield return Condition;
        yield return CloseParenthesis;
        yield return Then;
        yield return ElseToken;
        yield return Else;
    }
}