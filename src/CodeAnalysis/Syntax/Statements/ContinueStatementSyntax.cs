namespace CodeAnalysis.Syntax.Statements;

public sealed record class ContinueStatementSyntax(SyntaxToken ContinueKeyword, SyntaxToken SemicolonToken)
    : StatementSyntax(SyntaxKind.ContinueStatement)
{
    /// <inheritdoc />
    public override IEnumerable<SyntaxNode> Children()
    {
        yield return ContinueKeyword;
        yield return SemicolonToken;
    }
}
