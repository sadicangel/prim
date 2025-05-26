namespace CodeAnalysis.Syntax.Expressions;
public sealed record class LiteralExpressionSyntax(
    SyntaxKind SyntaxKind,
    SyntaxTree SyntaxTree,
    SyntaxToken LiteralToken,
    object InstanceValue)
    : ExpressionSyntax(SyntaxKind, SyntaxTree)
{
    public override IEnumerable<SyntaxNode> Children()
    {
        yield return LiteralToken;
    }
}
