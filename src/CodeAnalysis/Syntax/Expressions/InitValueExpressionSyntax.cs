namespace CodeAnalysis.Syntax.Expressions;

public sealed record class InitValueExpressionSyntax(
    SyntaxTree SyntaxTree,
    SyntaxToken ColonOrEqualsToken,
    ExpressionSyntax Expression)
    : ExpressionSyntax(SyntaxKind.InitValueExpression, SyntaxTree)
{
    public bool IsReadOnly { get => ColonOrEqualsToken.SyntaxKind is SyntaxKind.ColonToken; }

    public override IEnumerable<SyntaxNode> Children()
    {
        yield return ColonOrEqualsToken;
        yield return Expression;
    }
}
