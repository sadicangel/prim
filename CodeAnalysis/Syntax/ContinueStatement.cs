namespace CodeAnalysis.Syntax;

public sealed record class ContinueStatement(Token Continue) : Statement(SyntaxNodeKind.ContinueStatement)
{
    public override T Accept<T>(ISyntaxStatementVisitor<T> visitor) => visitor.Visit(this);
    public override IEnumerable<SyntaxNode> GetChildren()
    {
        yield return Continue;
    }
}
