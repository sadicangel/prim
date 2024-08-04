namespace CodeAnalysis.Syntax.Expressions;

public sealed record class InitValueExpressionSyntax(
    SyntaxTree SyntaxTree,
    SyntaxToken EqualsToken,
    ExpressionSyntax Expression)
    : ExpressionSyntax(SyntaxKind.InitValueExpression, SyntaxTree)
{
    public override IEnumerable<SyntaxNode> Children()
    {
        yield return EqualsToken;
        yield return Expression;
    }
}
