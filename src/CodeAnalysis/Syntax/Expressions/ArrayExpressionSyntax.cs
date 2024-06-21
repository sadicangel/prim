
namespace CodeAnalysis.Syntax.Expressions;
public sealed record class ArrayExpressionSyntax(
    SyntaxTree SyntaxTree,
    SyntaxToken BracketOpenToken,
    SeparatedSyntaxList<ExpressionSyntax> Elements,
    SyntaxToken BracketCloseToken)
    : ExpressionSyntax(SyntaxKind.ArrayExpression, SyntaxTree)
{
    public override IEnumerable<SyntaxNode> Children()
    {
        yield return BracketOpenToken;
        foreach (var node in Elements.SyntaxNodes)
            yield return node;
        yield return BracketCloseToken;
    }
}
