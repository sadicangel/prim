namespace CodeAnalysis.Syntax.Expressions;
public sealed record class BlockExpressionSyntax(
    SyntaxTree SyntaxTree,
    SyntaxToken BraceOpenToken,
    SyntaxList<ExpressionSyntax> Expressions,
    SyntaxToken BraceCloseToken)
    : ExpressionSyntax(SyntaxKind.BlockExpression, SyntaxTree)
{
    public override IEnumerable<SyntaxNode> Children()
    {
        yield return BraceOpenToken;
        foreach (var expression in Expressions)
            yield return expression;
        yield return BraceCloseToken;
    }
}
