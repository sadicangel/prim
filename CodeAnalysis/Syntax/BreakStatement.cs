namespace CodeAnalysis.Syntax;

public sealed record class BreakStatement(Token Break) : Statement(SyntaxNodeKind.BreakStatement)
{
    public override T Accept<T>(ISyntaxStatementVisitor<T> visitor) => visitor.Visit(this);
    public override IEnumerable<SyntaxNode> GetChildren()
    {
        yield return Break;
    }
}
