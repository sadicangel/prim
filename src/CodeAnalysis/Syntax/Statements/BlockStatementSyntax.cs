namespace CodeAnalysis.Syntax.Statements;

public sealed record class BlockStatementSyntax(
    SyntaxToken BraceOpenToken,
    SyntaxList<StatementSyntax> Statements,
    SyntaxToken BraceCloseToken)
    : StatementSyntax(SyntaxKind.BlockStatement)
{
    public override IEnumerable<SyntaxNode> Children()
    {
        yield return BraceOpenToken;
        foreach (var statement in Statements)
            yield return statement;
        yield return BraceCloseToken;
    }
}
