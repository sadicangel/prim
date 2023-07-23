namespace CodeAnalysis.Syntax.Expressions;

public sealed record class CompoundAssignmentExpression(SyntaxTree SyntaxTree, Token Identifier, Token Operator, Token Equal, Expression Expression)
    : Expression(SyntaxNodeKind.CompoundAssignmentExpression, SyntaxTree)
{
    public override T Accept<T>(ISyntaxExpressionVisitor<T> visitor) => visitor.Visit(this);
    public override IEnumerable<SyntaxNode> Children()
    {
        yield return Identifier;
        yield return Operator;
        yield return Equal;
        yield return Expression;
    }
    public override string ToString() => base.ToString();
}