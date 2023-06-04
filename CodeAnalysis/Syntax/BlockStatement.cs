namespace CodeAnalysis.Syntax;

public sealed record class BlockStatement(Token OpenBraceToken, IReadOnlyList<Statement> Statements, Token CloseBraceToken) : Statement(NodeKind.BlockStatement)
{
    public override T Accept<T>(IStatementVisitor<T> visitor) => visitor.Accept(this);
    public override IEnumerable<INode> GetChildren()
    {
        yield return OpenBraceToken;
        foreach (var statement in Statements)
            yield return statement;
        yield return CloseBraceToken;
    }
}
