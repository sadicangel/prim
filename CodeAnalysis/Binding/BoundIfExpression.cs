﻿namespace CodeAnalysis.Binding;

internal sealed record class BoundIfExpression(BoundExpression Condition, BoundExpression Then, BoundExpression Else, Type Type) : BoundExpression(BoundNodeKind.IfExpression, Type)
{
    public override T Accept<T>(IBoundExpressionVisitor<T> visitor) => visitor.Visit(this);
}