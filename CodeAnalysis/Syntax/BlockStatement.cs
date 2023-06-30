namespace CodeAnalysis.Syntax;

public sealed record class BlockStatement(Token OpenBrace, IReadOnlyList<Statement> Statements, Token CloseBrace) : Statement(SyntaxNodeKind.BlockStatement)
{
    public override T Accept<T>(ISyntaxStatementVisitor<T> visitor) => visitor.Visit(this);
    public override IEnumerable<SyntaxNode> GetChildren()
    {
        yield return OpenBrace;
        foreach (var statement in Statements)
            yield return statement;
        yield return CloseBrace;
    }
}