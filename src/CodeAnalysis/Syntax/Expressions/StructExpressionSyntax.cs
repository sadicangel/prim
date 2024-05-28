

namespace CodeAnalysis.Syntax.Expressions;
public sealed record class StructExpressionSyntax(
    SyntaxTree SyntaxTree,
    SyntaxToken BraceOpenToken,
    SeparatedSyntaxList<MemberExpressionSyntax> Members,
    SyntaxToken BraceCloseToken)
    : ExpressionSyntax(SyntaxKind.StructExpression, SyntaxTree)
{
    public override IEnumerable<SyntaxNode> Children()
    {
        yield return BraceOpenToken;
        foreach (var node in Members.SyntaxNodes)
            yield return node;
        yield return BraceCloseToken;
    }
}
