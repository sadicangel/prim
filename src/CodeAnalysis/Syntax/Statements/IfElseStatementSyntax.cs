using CodeAnalysis.Syntax.Expressions;

namespace CodeAnalysis.Syntax.Statements;

public sealed record class IfElseStatementSyntax(
    SyntaxToken IfKeyword,
    SyntaxToken ParenthesisOpenToken,
    ExpressionSyntax Condition,
    SyntaxToken ParenthesisCloseToken,
    StatementSyntax Then,
    ElseClauseSyntax? ElseClause)
    : StatementSyntax(SyntaxKind.IfElseStatement)
{
    public override IEnumerable<SyntaxNode> Children()
    {
        yield return IfKeyword;
        yield return ParenthesisOpenToken;
        yield return Condition;
        yield return ParenthesisCloseToken;
        yield return Then;
        if (ElseClause is not null)
            yield return ElseClause;
    }
}
