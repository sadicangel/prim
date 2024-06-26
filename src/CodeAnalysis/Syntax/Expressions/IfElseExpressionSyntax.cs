
namespace CodeAnalysis.Syntax.Expressions;
public sealed record class IfElseExpressionSyntax(
    SyntaxTree SyntaxTree,
    SyntaxToken IfKeyword,
    SyntaxToken ParenthesisOpenToken,
    ExpressionSyntax Condition,
    SyntaxToken ParenthesisCloseToken,
    ExpressionSyntax Then,
    SyntaxToken ElseKeyword,
    ExpressionSyntax Else)
    : ExpressionSyntax(SyntaxKind.IfElseExpression, SyntaxTree)
{
    public override IEnumerable<SyntaxNode> Children()
    {
        yield return IfKeyword;
        yield return ParenthesisOpenToken;
        yield return Condition;
        yield return ParenthesisCloseToken;
        yield return Then;
        yield return ElseKeyword;
        yield return Else;
    }
}
