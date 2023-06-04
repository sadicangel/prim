namespace CodeAnalysis.Syntax;

public sealed record class ExpressionStatement(Expression Expression) : Statement(NodeKind.ExpressionStatement)
{
    public override T Accept<T>(IStatementVisitor<T> visitor) => visitor.Accept(this);
    public override IEnumerable<INode> GetChildren()
    {
        yield return Expression;
    }
}
