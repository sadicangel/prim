namespace CodeAnalysis.Syntax;

public sealed record class GroupExpression(SyntaxTree SyntaxTree, Token OpenParenthesis, Expression Expression, Token CloseParenthesis)
    : Expression(SyntaxNodeKind.GroupExpression, SyntaxTree)
{
    public override T Accept<T>(ISyntaxExpressionVisitor<T> visitor) => visitor.Visit(this);
    public override IEnumerable<SyntaxNode> GetChildren()
    {
        yield return OpenParenthesis;
        yield return Expression;
        yield return CloseParenthesis;
    }
}
