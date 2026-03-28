namespace CodeAnalysis.Syntax.Expressions;

public sealed record class LiteralExpressionSyntax(
    SyntaxKind SyntaxKind,
    SyntaxToken LiteralToken,
    object InstanceValue)
    : ExpressionSyntax(SyntaxKind)
{
    public override IEnumerable<SyntaxNode> Children()
    {
        yield return LiteralToken;
    }
}
