

namespace CodeAnalysis.Syntax.Expressions;
public sealed record class StructExpressionSyntax(
    SyntaxTree SyntaxTree,
    SyntaxToken BraceOpenToken,
    SeparatedSyntaxList<PropertyExpressionSyntax> Properties,
    SyntaxToken BraceCloseToken)
    : ExpressionSyntax(SyntaxKind.StructExpression, SyntaxTree)
{
    public override IEnumerable<SyntaxNode> Children()
    {
        yield return BraceOpenToken;
        foreach (var node in Properties.SyntaxNodes)
            yield return node;
        yield return BraceCloseToken;
    }
}
