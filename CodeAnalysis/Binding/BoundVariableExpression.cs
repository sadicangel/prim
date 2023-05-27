﻿namespace CodeAnalysis.Binding;

public sealed record class BoundVariableExpression(Variable Variable) : BoundExpression(BoundNodeKind.VariableExpression, Variable.Type)
{
    public override T Accept<T>(IBoundExpressionVisitor<T> visitor) => visitor.Visit(this);
}
