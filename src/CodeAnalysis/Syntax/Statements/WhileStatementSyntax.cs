using CodeAnalysis.Syntax.Expressions;

namespace CodeAnalysis.Syntax.Statements;

public sealed record class WhileStatementSyntax(
    SyntaxToken WhileKeyword,
    SyntaxToken ParenthesisOpenToken,
    ExpressionSyntax Condition,
    SyntaxToken ParenthesisCloseToken,
    StatementSyntax Body)
    : StatementSyntax(SyntaxKind.WhileStatement)
{
    /// <inheritdoc />
    public override IEnumerable<SyntaxNode> Children()
    {
        yield return WhileKeyword;
        yield return ParenthesisOpenToken;
        yield return Condition;
        yield return ParenthesisCloseToken;
        yield return Body;
    }
}
