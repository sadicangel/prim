namespace CodeAnalysis.Syntax.Expressions;

public sealed record class UnaryExpressionSyntax(
    SyntaxKind SyntaxKind,
    SyntaxToken OperatorToken,
    ExpressionSyntax Operand)
    : ExpressionSyntax(SyntaxKind)
{
    public override IEnumerable<SyntaxNode> Children()
    {
        yield return OperatorToken;
        yield return Operand;
    }
}
