namespace CodeAnalysis.Syntax.Expressions.ControlFlow;
public sealed record class IfExpressionSyntax(
    SyntaxTree SyntaxTree,
    SyntaxToken IfKeyword,
    SyntaxToken ParenthesisOpenToken,
    ExpressionSyntax Condition,
    SyntaxToken ParenthesisCloseToken,
    ExpressionSyntax Then,
    ElseClauseExpressionSyntax? ElseClause)
    : ExpressionSyntax(SyntaxKind.IfExpression, SyntaxTree)
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
