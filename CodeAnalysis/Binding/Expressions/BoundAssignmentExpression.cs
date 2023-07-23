using CodeAnalysis.Symbols;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Expressions;

internal sealed record class BoundAssignmentExpression(SyntaxNode Syntax, VariableSymbol Variable, BoundExpression Expression)
    : BoundExpression(BoundNodeKind.AssignmentExpression, Syntax, Expression.Type)
{
    public override T Accept<T>(IBoundExpressionVisitor<T> visitor) => visitor.Visit(this);
    public override IEnumerable<INode> Descendants()
    {
        yield return Variable;
        yield return Expression;
    }
}