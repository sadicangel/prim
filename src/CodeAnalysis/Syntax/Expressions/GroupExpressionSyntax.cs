namespace CodeAnalysis.Syntax.Expressions;

public sealed record class GroupExpressionSyntax(
    SyntaxToken ParenthesisOpenToken,
    ExpressionSyntax Expression,
    SyntaxToken ParenthesisCloseToken)
    : ExpressionSyntax(SyntaxKind.GroupExpression)
{
    public override IEnumerable<SyntaxNode> Children()
    {
        yield return ParenthesisOpenToken;
        yield return Expression;
        yield return ParenthesisCloseToken;
    }
}
