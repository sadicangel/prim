using System.Diagnostics.CodeAnalysis;

namespace CodeAnalysis.Binding;

internal sealed record class BoundIfStatement(BoundExpression Condition, BoundStatement Then, BoundStatement? Else) : BoundStatement(BoundNodeKind.IfStatement)
{
    public BoundIfStatement(BoundExpression condition, BoundStatement then) : this(condition, then, Else: null) { }

    [MemberNotNullWhen(true, nameof(Else))]
    public bool HasElseClause { get => Else is not null; }

    public override T Accept<T>(IBoundStatementVisitor<T> visitor) => visitor.Visit(this);
    public override IEnumerable<INode> GetChildren()
    {
        yield return Condition;
        yield return Then;
        if (Else is not null)
            yield return Else;
    }
}