namespace CodeAnalysis.Syntax.Statements;

public sealed record class EmptyStatementSyntax(SyntaxToken SemicolonToken)
    : StatementSyntax(SyntaxKind.EmptyStatement)
{
    public override IEnumerable<SyntaxNode> Children()
    {
        yield return SemicolonToken;
    }
}
