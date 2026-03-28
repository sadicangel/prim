namespace CodeAnalysis.Syntax.Expressions;

public sealed record class EmptyExpressionSyntax(SyntaxToken SemicolonToken)
    : ExpressionSyntax(SyntaxKind.EmptyExpression)
{
    public override IEnumerable<SyntaxNode> Children()
    {
        yield return SemicolonToken;
    }
}
