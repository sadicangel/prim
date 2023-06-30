﻿namespace CodeAnalysis.Binding;

internal sealed record class BoundExpressionStatement(BoundExpression Expression) : BoundStatement(BoundNodeKind.ExpressionStatement)
{
    public override T Accept<T>(IBoundStatementVisitor<T> visitor) => visitor.Visit(this);
    public override IEnumerable<INode> GetChildren()
    {
        yield return Expression;
    }
}
