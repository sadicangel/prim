using CodeAnalysis.Symbols;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Expressions;
internal sealed record class BoundConvertExpression(SyntaxNode Syntax, BoundExpression Expression, TypeSymbol Type)
    : BoundExpression(BoundNodeKind.ConvertExpression, Syntax, Type)
{
    public override T Accept<T>(IBoundExpressionVisitor<T> visitor) => visitor.Visit(this);
    public override IEnumerable<INode> Children()
    {
        yield return Expression;
    }
}
