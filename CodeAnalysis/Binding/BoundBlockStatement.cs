﻿namespace CodeAnalysis.Binding;

internal sealed record class BoundBlockStatement(IReadOnlyList<BoundStatement> Statements) : BoundStatement(BoundNodeKind.BlockStatement)
{
    public override void Accept(IBoundStatementVisitor visitor) => visitor.Accept(this);
    public override IEnumerable<INode> GetChildren() => Statements;
}