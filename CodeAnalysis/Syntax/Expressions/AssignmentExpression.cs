namespace CodeAnalysis.Syntax.Expressions;

public sealed record class AssignmentExpression(SyntaxTree SyntaxTree, Token Identifier, Token Assign, Expression Expression)
    : Expression(SyntaxNodeKind.AssignmentExpression, SyntaxTree)
{
    public override T Accept<T>(ISyntaxExpressionVisitor<T> visitor) => visitor.Visit(this);
    public override IEnumerable<SyntaxNode> Children()
    {
        yield return Identifier;
        yield return Assign;
        yield return Expression;
    }
    public override string ToString() => base.ToString();
}