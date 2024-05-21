
namespace CodeAnalysis.Syntax.Expressions;
public sealed record class GroupExpressionSyntax(
    SyntaxTree SyntaxTree,
    SyntaxToken ParenthesisOpenToken,
    ExpressionSyntax Expression,
    SyntaxToken ParenthesisCloseToken)
    : ExpressionSyntax(SyntaxKind.GroupExpression, SyntaxTree)
{
    public override IEnumerable<SyntaxNode> Children()
    {
        yield return ParenthesisOpenToken;
        yield return Expression;
        yield return ParenthesisCloseToken;
    }
}
