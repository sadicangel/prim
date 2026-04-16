namespace CodeAnalysis.Syntax.Statements;

public sealed record class ElseClauseSyntax(
    SyntaxToken ElseKeyword,
    StatementSyntax Else)
    : StatementSyntax(SyntaxKind.ElseClause)
{
    public override IEnumerable<SyntaxNode> Children()
    {
        yield return ElseKeyword;
        yield return Else;
    }
}
