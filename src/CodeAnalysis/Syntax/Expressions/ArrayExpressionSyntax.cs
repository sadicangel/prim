namespace CodeAnalysis.Syntax.Expressions;

public sealed record class ArrayExpressionSyntax(
    SyntaxToken BracketOpenToken,
    SeparatedSyntaxList<ExpressionSyntax> Elements,
    SyntaxToken BracketCloseToken)
    : ExpressionSyntax(SyntaxKind.ArrayExpression)
{
    public override IEnumerable<SyntaxNode> Children()
    {
        yield return BracketOpenToken;
        foreach (var node in Elements.SyntaxNodes)
            yield return node;
        yield return BracketCloseToken;
    }
}
