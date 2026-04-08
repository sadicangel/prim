using CodeAnalysis.Syntax.Expressions;

namespace CodeAnalysis.Syntax.ControlFlow;

public sealed record class IfElseExpressionSyntax(
    SyntaxToken IfKeyword,
    SyntaxToken ParenthesisOpenToken,
    ExpressionSyntax Condition,
    SyntaxToken ParenthesisCloseToken,
    ExpressionSyntax Then,
    ElseClauseSyntax? ElseClause)
    : ExpressionSyntax(SyntaxKind.IfElseExpression)
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
