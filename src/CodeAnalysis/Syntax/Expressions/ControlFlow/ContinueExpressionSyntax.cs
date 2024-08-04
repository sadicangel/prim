namespace CodeAnalysis.Syntax.Expressions.ControlFlow;
public sealed record class ContinueExpressionSyntax(
    SyntaxTree SyntaxTree,
    SyntaxToken ContinueKeyword,
    ExpressionSyntax? Expression,
    SyntaxToken? SemicolonToken)
    : ExpressionSyntax(SyntaxKind.ContinueExpression, SyntaxTree)
{
    public override IEnumerable<SyntaxNode> Children()
    {
        yield return ContinueKeyword;
        if (Expression is not null)
            yield return Expression;
        if (SemicolonToken is not null)
            yield return SemicolonToken;
    }
}
