namespace CodeAnalysis.Syntax;

public sealed record class WhileStatement(Token WhileToken, Expression Condition, Statement Body) : Statement(SyntaxNodeKind.WhileStatement)
{
    public override T Accept<T>(ISyntaxStatementVisitor<T> visitor) => visitor.Visit(this);
    public override IEnumerable<SyntaxNode> GetChildren()
    {
        yield return WhileToken;
        yield return Condition;
        yield return Body;
    }
}