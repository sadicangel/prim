namespace CodeAnalysis.Syntax.Expressions.ControlFlow;

public sealed record class BreakExpressionSyntax(
    SyntaxTree SyntaxTree,
    SyntaxToken BreakKeyword,
    ExpressionSyntax? Expression,
    SyntaxToken? SemicolonToken)
    : ExpressionSyntax(SyntaxKind.BreakExpression, SyntaxTree)
{
    public override IEnumerable<SyntaxNode> Children()
    {
        yield return BreakKeyword;
        if (Expression is not null)
            yield return Expression;
        if (SemicolonToken is not null)
            yield return SemicolonToken;
    }
}
