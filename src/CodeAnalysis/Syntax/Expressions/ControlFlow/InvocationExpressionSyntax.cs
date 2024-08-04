namespace CodeAnalysis.Syntax.Expressions.ControlFlow;
public sealed record class InvocationExpressionSyntax(
    SyntaxTree SyntaxTree,
    ExpressionSyntax Expression,
    SyntaxToken ParenthesisOpenToken,
    SeparatedSyntaxList<ArgumentSyntax> Arguments,
    SyntaxToken ParenthesisCloseToken)
    : ExpressionSyntax(SyntaxKind.InvocationExpression, SyntaxTree)

{
    public override IEnumerable<SyntaxNode> Children()
    {
        yield return Expression;
        yield return ParenthesisOpenToken;
        foreach (var node in Arguments.SyntaxNodes)
            yield return node;
        yield return ParenthesisCloseToken;
    }
}
