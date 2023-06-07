﻿namespace CodeAnalysis.Binding;

internal sealed record class BoundExpressionStatement(BoundExpression Expression) : BoundStatement(BoundNodeKind.ExpressionStatement)
{
    public override void Accept(IBoundStatementVisitor visitor) => visitor.Accept(this);
}