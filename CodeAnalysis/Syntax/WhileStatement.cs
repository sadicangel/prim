namespace CodeAnalysis.Syntax;

public sealed record class WhileStatement(Token WhileToken, Expression Condition, Statement Body) : Statement(NodeKind.WhileStatement)
{
    public override T Accept<T>(IStatementVisitor<T> visitor) => visitor.Accept(this);
    public override IEnumerable<Node> GetChildren()
    {
        yield return WhileToken;
        yield return Condition;
        yield return Body;
    }
}