
namespace CodeAnalysis.Syntax.Expressions;
public sealed record class ArrayExpressionSyntax(
    SyntaxTree SyntaxTree,
    SyntaxToken BracketOpenToken,
    SeparatedSyntaxList<ExpressionSyntax> Expressions,
    SyntaxToken BracketCloseToken)
    : ExpressionSyntax(SyntaxKind.ArrayExpression, SyntaxTree)
{
    public override IEnumerable<SyntaxNode> Children()
    {
        yield return BracketOpenToken;
        foreach (var node in Expressions.SyntaxNodes)
            yield return node;
        yield return BracketCloseToken;
    }
}
