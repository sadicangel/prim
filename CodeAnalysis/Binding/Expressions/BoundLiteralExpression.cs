﻿using CodeAnalysis.Symbols;

namespace CodeAnalysis.Binding.Expressions;

internal sealed record class BoundLiteralExpression(object? Value) : BoundExpression(BoundNodeKind.LiteralExpression, TypeSymbol.TypeOf(Value))
{
    public object? Value { get => ConstantValue.Value; }
    public override ConstantValue ConstantValue { get; } = new ConstantValue(Value);
    public override T Accept<T>(IBoundExpressionVisitor<T> visitor) => visitor.Visit(this);
    public override IEnumerable<INode> GetChildren() => Enumerable.Empty<INode>();
}
