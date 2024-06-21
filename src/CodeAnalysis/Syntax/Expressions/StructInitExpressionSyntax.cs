

namespace CodeAnalysis.Syntax.Expressions;
public sealed record class StructInitExpressionSyntax(
    SyntaxTree SyntaxTree,
    SyntaxToken IdentifierToken,
    SyntaxToken BraceOpenToken,
    SeparatedSyntaxList<PropertyInitExpressionSyntax> Properties,
    SyntaxToken BraceCloseToken)
    : ExpressionSyntax(SyntaxKind.StructInitExpression, SyntaxTree)
{
    public override IEnumerable<SyntaxNode> Children()
    {
        yield return IdentifierToken;
        yield return BraceOpenToken;
        foreach (var node in Properties.SyntaxNodes)
            yield return node;
        yield return BraceCloseToken;
    }
}
