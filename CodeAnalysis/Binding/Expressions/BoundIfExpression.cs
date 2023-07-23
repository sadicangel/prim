using CodeAnalysis.Symbols;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Expressions;

internal sealed record class BoundIfExpression(SyntaxNode Syntax, BoundExpression Condition, BoundExpression Then, BoundExpression Else, TypeSymbol Type)
    : BoundExpression(BoundNodeKind.IfExpression, Syntax, Type)
{
    public override T Accept<T>(IBoundExpressionVisitor<T> visitor) => visitor.Visit(this);
    public override IEnumerable<INode> Descendants()
    {
        yield return Condition;
        yield return Then;
        yield return Else;
    }
}