namespace CodeAnalysis.Syntax.Expressions;

public sealed record class StatementExpressionSyntax(ExpressionSyntax Expression, SyntaxToken SemicolonToken)
    : ExpressionSyntax(SyntaxKind.StatementExpression)
{
    public override IEnumerable<SyntaxNode> Children()
    {
        yield return Expression;
        yield return SemicolonToken;
    }
}
