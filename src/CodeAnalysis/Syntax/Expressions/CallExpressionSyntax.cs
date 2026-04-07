namespace CodeAnalysis.Syntax.Expressions;

public sealed record class CallExpressionSyntax(
    ExpressionSyntax Callee,
    SyntaxToken ParenthesisOpenToken,
    SeparatedSyntaxList<ExpressionSyntax> Arguments,
    SyntaxToken ParenthesisCloseToken)
    : ExpressionSyntax(SyntaxKind.InvocationExpression)
{
    /// <inheritdoc />
    public override IEnumerable<SyntaxNode> Children()
    {
        yield return Callee;
        yield return ParenthesisOpenToken;
        foreach (var argument in Arguments)
            yield return argument;
        yield return ParenthesisCloseToken;
    }
}
