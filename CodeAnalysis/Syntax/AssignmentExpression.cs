namespace CodeAnalysis.Syntax;

public sealed record class AssignmentExpression(SyntaxTree SyntaxTree, Token Identifier, Token Equal, Expression Expression)
    : Expression(SyntaxNodeKind.AssignmentExpression, SyntaxTree)
{
    public override T Accept<T>(ISyntaxExpressionVisitor<T> visitor) => visitor.Visit(this);
    public override IEnumerable<SyntaxNode> GetChildren()
    {
        yield return Identifier;
        yield return Equal;
        yield return Expression;
    }
}
