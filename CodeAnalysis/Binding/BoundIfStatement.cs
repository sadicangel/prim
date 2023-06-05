using System.Diagnostics.CodeAnalysis;

namespace CodeAnalysis.Binding;

internal sealed record class BoundIfStatement(BoundExpression Condition, BoundStatement Then, BoundStatement? Else) : BoundStatement(BoundNodeKind.IfStatement)
{
    public BoundIfStatement(BoundExpression condition, BoundStatement then) : this(condition, then, Else: null) { }

    [MemberNotNullWhen(true, nameof(Else))]
    public bool HasElseClause { get => Else is not null; }

    public override void Accept(IBoundStatementVisitor visitor) => visitor.Accept(this);
}