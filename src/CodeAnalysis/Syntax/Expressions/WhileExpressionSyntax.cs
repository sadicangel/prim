
namespace CodeAnalysis.Syntax.Expressions;
public sealed record class WhileExpressionSyntax(
    SyntaxTree SyntaxTree,
    SyntaxToken WhileKeyword,
    SyntaxToken ParenthesisOpenToken,
    ExpressionSyntax Condition,
    SyntaxToken ParenthesisCloseToken,
    ExpressionSyntax Body)
    : ExpressionSyntax(SyntaxKind.WhileExpression, SyntaxTree)
{
    public override IEnumerable<SyntaxNode> Children()
    {
        yield return WhileKeyword;
        yield return ParenthesisOpenToken;
        yield return Condition;
        yield return ParenthesisCloseToken;
        yield return Body;
    }
}
