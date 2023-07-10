using CodeAnalysis.Symbols;

namespace CodeAnalysis.Binding.Expressions;
internal sealed record class BoundCallExpression(FunctionSymbol Function, IReadOnlyList<BoundExpression> Arguments) : BoundExpression(BoundNodeKind.CallExpression, Function.Type)
{
    public override T Accept<T>(IBoundExpressionVisitor<T> visitor) => visitor.Visit(this);
    public override IEnumerable<INode> GetChildren()
    {
        foreach (var expression in Arguments)
            yield return expression;
    }
}