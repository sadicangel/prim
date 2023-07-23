using CodeAnalysis.Symbols;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Expressions;
internal sealed record class BoundCallExpression(SyntaxNode Syntax, FunctionSymbol Function, IReadOnlyList<BoundExpression> Arguments)
    : BoundExpression(BoundNodeKind.CallExpression, Syntax, Function.Type)
{
    public override T Accept<T>(IBoundExpressionVisitor<T> visitor) => visitor.Visit(this);
    public override IEnumerable<INode> Descendants()
    {
        foreach (var expression in Arguments)
            yield return expression;
    }
}