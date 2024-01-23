using CodeAnalysis.Symbols;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Expressions;

internal sealed record class BoundLiteralExpression(SyntaxNode Syntax, object? Value)
    : BoundExpression(BoundNodeKind.LiteralExpression, Syntax, TypeSymbol.TypeOf(Value))
{
    public object? Value { get => ConstantValue.Value; }
    public override ConstantValue ConstantValue { get; } = new ConstantValue(Value);
    public override T Accept<T>(IBoundExpressionVisitor<T> visitor) => visitor.Visit(this);
    public override IEnumerable<INode> Children() => Enumerable.Empty<INode>();
}
