using CodeAnalysis.Syntax.Expressions;

namespace CodeAnalysis.Syntax.ControlFlow;

public sealed record class WhileExpressionSyntax(
    SyntaxToken WhileKeyword,
    SyntaxToken ParenthesisOpenToken,
    ExpressionSyntax Condition,
    SyntaxToken ParenthesisCloseToken,
    ExpressionSyntax Body)
    : ExpressionSyntax(SyntaxKind.WhileExpression)
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
