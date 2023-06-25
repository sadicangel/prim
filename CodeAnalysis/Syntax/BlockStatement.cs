namespace CodeAnalysis.Syntax;

public sealed record class BlockStatement(Token OpenBraceToken, IReadOnlyList<Statement> Statements, Token CloseBraceToken) : Statement(SyntaxNodeKind.BlockStatement)
{
    public override T Accept<T>(ISyntaxStatementVisitor<T> visitor) => visitor.Visit(this);
    public override IEnumerable<SyntaxNode> GetChildren()
    {
        yield return OpenBraceToken;
        foreach (var statement in Statements)
            yield return statement;
        yield return CloseBraceToken;
    }
}
